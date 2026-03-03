using ManuBackend.DTOs;
using ManuBackend.Models;
using ManuBackend.Repository;

namespace ManuBackend.Services
{
    // SERVICE IMPLEMENTATION
    // ----------------------
    // InventoryService contains BUSINESS LOGIC
    // It sits between Controller and Repository
    public class InventoryService : IInventoryService
    {
        // Repository for inventory-related DB operations
        private readonly IInventoryRepository _inventoryRepo;

        // Repository for product & BOM data
        private readonly IProductRepository _productRepo;

        // Predefined warehouse locations
        // Used while auto-generating inventories
        private readonly string[] _locations =
        {
            "Chennai",
            "Coimbatore",
            "Bangalore",
            "Hyderabad"
        };

        // Constructor Dependency Injection
        public InventoryService(
            IInventoryRepository inventoryRepo,
            IProductRepository productRepo)
        {
            _inventoryRepo = inventoryRepo;
            _productRepo = productRepo;
        }

        // ============================================================
        // READ OPERATIONS
        // ============================================================

        // Get all inventories
        // Auto-generates inventories if none exist
        public async Task<List<InventoryDto>> GetAllInventoriesAsync()
        {
            // Fetch inventories from database
            var inventories = await _inventoryRepo.GetAllInventoriesAsync();

            // SAFETY LOGIC:
            // If inventory table is empty, generate inventory automatically
            if (!inventories.Any())
            {
                await GenerateInventoriesAsync();
                inventories = await _inventoryRepo.GetAllInventoriesAsync();
            }

            // Convert Entity models → DTOs
            return inventories.Select(i => new InventoryDto
            {
                InventoryId = i.InventoryId,
                ProductId = i.ProductId,
                Location = i.Location,
                CreatedAt = i.CreatedAt,

                // Nested mapping for materials
                Materials = i.Materials.Select(m => new InventoryMaterialDto
                {
                    Id = m.Id,
                    InventoryId = m.InventoryId,
                    MaterialName = m.MaterialName,
                    AvailableQty = m.AvailableQty,
                    ThresholdQty = m.ThresholdQty,
                    CreatedAt = m.CreatedAt
                }).ToList()
            }).ToList();
        }

        // Get inventory by InventoryId
        public async Task<InventoryDto?> GetInventoryByIdAsync(int inventoryId)
        {
            var inventory = await _inventoryRepo.GetInventoryByIdAsync(inventoryId);
            if (inventory == null) return null;

            return new InventoryDto
            {
                InventoryId = inventory.InventoryId,
                ProductId = inventory.ProductId,
                Location = inventory.Location,
                CreatedAt = inventory.CreatedAt,
                Materials = inventory.Materials.Select(m => new InventoryMaterialDto
                {
                    Id = m.Id,
                    InventoryId = m.InventoryId,
                    MaterialName = m.MaterialName,
                    AvailableQty = m.AvailableQty,
                    ThresholdQty = m.ThresholdQty,
                    CreatedAt = m.CreatedAt
                }).ToList()
            };
        }

        // Get inventories by ProductId
        public async Task<List<InventoryDto>> GetInventoriesByProductIdAsync(int productId)
        {
            var inventories = await _inventoryRepo.GetInventoriesByProductIdAsync(productId);

            return inventories.Select(i => new InventoryDto
            {
                InventoryId = i.InventoryId,
                ProductId = i.ProductId,
                Location = i.Location,
                CreatedAt = i.CreatedAt,
                Materials = i.Materials.Select(m => new InventoryMaterialDto
                {
                    Id = m.Id,
                    InventoryId = m.InventoryId,
                    MaterialName = m.MaterialName,
                    AvailableQty = m.AvailableQty,
                    ThresholdQty = m.ThresholdQty,
                    CreatedAt = m.CreatedAt
                }).ToList()
            }).ToList();
        }

        // ============================================================
        // UPDATE OPERATIONS
        // ============================================================

        // Update material quantities & thresholds
        public async Task<InventoryMaterialDto> UpdateMaterialAsync(
            int materialId,
            UpdateInventoryMaterialDto dto)
        {
            // Fetch material
            var material = await _inventoryRepo.GetMaterialByIdAsync(materialId);
            if (material == null)
                throw new KeyNotFoundException($"Material {materialId} not found");

            // Update only provided fields
            if (dto.AvailableQty.HasValue)
                material.AvailableQty = dto.AvailableQty.Value;

            if (dto.ThresholdQty.HasValue)
                material.ThresholdQty = dto.ThresholdQty.Value;

            // Persist changes
            var updated = await _inventoryRepo.UpdateMaterialAsync(material);

            return new InventoryMaterialDto
            {
                Id = updated.Id,
                InventoryId = updated.InventoryId,
                MaterialName = updated.MaterialName,
                AvailableQty = updated.AvailableQty,
                ThresholdQty = updated.ThresholdQty,
                CreatedAt = updated.CreatedAt
            };
        }

        // Adjust material quantity (+ / -)
        public async Task<InventoryMaterialDto> AdjustMaterialQuantityAsync(
            int materialId,
            int delta)
        {
            var material = await _inventoryRepo.GetMaterialByIdAsync(materialId);
            if (material == null)
                throw new KeyNotFoundException($"Material {materialId} not found");

            // Prevent negative stock
            material.AvailableQty = Math.Max(0, material.AvailableQty + delta);

            var updated = await _inventoryRepo.UpdateMaterialAsync(material);

            return new InventoryMaterialDto
            {
                Id = updated.Id,
                InventoryId = updated.InventoryId,
                MaterialName = updated.MaterialName,
                AvailableQty = updated.AvailableQty,
                ThresholdQty = updated.ThresholdQty,
                CreatedAt = updated.CreatedAt
            };
        }

        // Get materials below threshold
        public async Task<List<InventoryMaterialDto>> GetLowStockMaterialsAsync()
        {
            var materials = await _inventoryRepo.GetLowStockMaterialsAsync();

            return materials.Select(m => new InventoryMaterialDto
            {
                Id = m.Id,
                InventoryId = m.InventoryId,
                MaterialName = m.MaterialName,
                AvailableQty = m.AvailableQty,
                ThresholdQty = m.ThresholdQty,
                CreatedAt = m.CreatedAt
            }).ToList();
        }

        // ============================================================
        // ADMIN OPERATIONS
        // ============================================================

        // Auto-generate inventory for ACTIVE products
        public async Task<int> GenerateInventoriesAsync()
        {
            var products = await _productRepo.GetAllProductsAsync();
            var activeProducts = products
                .Where(p => p.Status == "ACTIVE")
                .ToList();

            int count = 0;
            var random = new Random();

            foreach (var product in activeProducts)
            {
                // Check if inventory already exists
                var existing = await _inventoryRepo
                    .GetInventoriesByProductIdAsync(product.Id);

                if (!existing.Any())
                {
                    var randomLocation =
                        _locations[random.Next(_locations.Length)];

                    var inventory = new Inventory
                    {
                        ProductId = product.Id,
                        Location = randomLocation,
                        CreatedAt = DateTime.UtcNow,

                        // Create inventory materials from BOM
                        Materials = product.BOMs.Select(bom =>
                            new InventoryMaterial
                            {
                                MaterialName = bom.MaterialName,
                                AvailableQty = random.Next(0, 30),
                                ThresholdQty = 10,
                                CreatedAt = DateTime.UtcNow
                            }).ToList()
                    };

                    await _inventoryRepo.CreateInventoryAsync(inventory);
                    count++;
                }
            }

            return count;
        }

        // Create inventory for a single product
        public async Task CreateInventoryForProductAsync(int productId)
        {
            var product = await _productRepo.GetProductByIdAsync(productId);
            if (product == null || product.Status != "ACTIVE")
                return;

            var existing =
                await _inventoryRepo.GetInventoriesByProductIdAsync(productId);
            if (existing.Any()) return;

            var random = new Random();
            var randomLocation =
                _locations[random.Next(_locations.Length)];

            var inventory = new Inventory
            {
                ProductId = product.Id,
                Location = randomLocation,
                CreatedAt = DateTime.UtcNow,
                Materials = product.BOMs.Select(bom =>
                    new InventoryMaterial
                    {
                        MaterialName = bom.MaterialName,
                        AvailableQty = random.Next(0, 30),
                        ThresholdQty = 10,
                        CreatedAt = DateTime.UtcNow
                    }).ToList()
            };

            await _inventoryRepo.CreateInventoryAsync(inventory);
        }

        // Sync inventory materials with BOM
        public async Task SyncInventoryMaterialsAsync(int productId)
        {
            var product = await _productRepo.GetProductByIdAsync(productId);
            if (product == null)
                throw new KeyNotFoundException($"Product {productId} not found");

            var inventories =
                await _inventoryRepo.GetInventoriesByProductIdAsync(productId);

            foreach (var inventory in inventories)
            {
                var freshInventory =
                    await _inventoryRepo.GetInventoryByIdAsync(inventory.InventoryId);
                if (freshInventory == null) continue;

                var bomMaterials =
                    product.BOMs.Select(b => b.MaterialName).ToList();

                var existingMaterials =
                    freshInventory.Materials.Select(m => m.MaterialName).ToList();

                // Add missing materials
                foreach (var materialName in bomMaterials.Except(existingMaterials))
                {
                    await _inventoryRepo.CreateMaterialAsync(
                        new InventoryMaterial
                        {
                            InventoryId = inventory.InventoryId,
                            MaterialName = materialName,
                            AvailableQty = new Random().Next(0, 30),
                            ThresholdQty = 10,
                            CreatedAt = DateTime.UtcNow
                        });
                }

                // Remove obsolete materials
                foreach (var materialName in existingMaterials.Except(bomMaterials))
                {
                    var material =
                        freshInventory.Materials
                        .FirstOrDefault(m => m.MaterialName == materialName);

                    if (material != null)
                        await _inventoryRepo.DeleteMaterialAsync(material.Id);
                }
            }
        }
    }
}