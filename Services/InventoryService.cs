using ManuBackend.DTOs;
using ManuBackend.Models;
using ManuBackend.Repository;

using ManuBackend.DTOs;
using ManuBackend.Models;
using ManuBackend.Repository;

namespace ManuBackend.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepo;
        private readonly IProductRepository _productRepo;

        private readonly string[] _locations = { "Chennai", "Coimbatore", "Bangalore", "Hyderabad" };

        public InventoryService(IInventoryRepository inventoryRepo, IProductRepository productRepo)
        {
            _inventoryRepo = inventoryRepo;
            _productRepo = productRepo;
        }

        // -------------------- READ OPERATIONS --------------------

        public async Task<List<InventoryDto>> GetAllInventoriesAsync()
        {
            var inventories = await _inventoryRepo.GetAllInventoriesAsync();

            if (!inventories.Any())
            {
                await GenerateInventoriesAsync();
                inventories = await _inventoryRepo.GetAllInventoriesAsync();
            }

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

        // -------------------- UPDATE OPERATIONS --------------------

        public async Task<InventoryMaterialDto> UpdateMaterialAsync(int materialId, UpdateInventoryMaterialDto dto)
        {
            var material = await _inventoryRepo.GetMaterialByIdAsync(materialId);
            if (material == null)
                throw new KeyNotFoundException($"Material {materialId} not found");

            if (dto.AvailableQty.HasValue)
                material.AvailableQty = dto.AvailableQty.Value;

            if (dto.ThresholdQty.HasValue)
                material.ThresholdQty = dto.ThresholdQty.Value;

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

        public async Task<InventoryMaterialDto> AdjustMaterialQuantityAsync(int materialId, int delta)
        {
            var material = await _inventoryRepo.GetMaterialByIdAsync(materialId);
            if (material == null)
                throw new KeyNotFoundException($"Material {materialId} not found");

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

        // -------------------- ADMIN OPERATIONS --------------------

        // ✅ FIXED: Only creates ONE inventory per product at random location
        public async Task<int> GenerateInventoriesAsync()
        {
            var products = await _productRepo.GetAllProductsAsync();
            var activeProducts = products.Where(p => p.Status == "ACTIVE").ToList();

            int count = 0;
            var random = new Random();

            foreach (var product in activeProducts)
            {
                // ✅ FIX: Check if product has ANY inventory
                var existing = await _inventoryRepo.GetInventoriesByProductIdAsync(product.Id);

                // Only create if product has NO inventory at all
                if (!existing.Any())
                {
                    // Pick one random location
                    var randomLocation = _locations[random.Next(_locations.Length)];

                    var inventory = new Inventory
                    {
                        ProductId = product.Id,
                        Location = randomLocation,
                        CreatedAt = DateTime.UtcNow,
                        Materials = product.BOMs.Select(bom => new InventoryMaterial
                        {
                            MaterialName = bom.MaterialName,
                            AvailableQty = 0,  // ✅ Start at 0
                            ThresholdQty = 0,  // ✅ Start at 0
                            CreatedAt = DateTime.UtcNow
                        }).ToList()
                    };

                    await _inventoryRepo.CreateInventoryAsync(inventory);
                    count++;
                }
            }

            return count;
        }

        // ✅ NEW: Create inventory for a single product (called when new product created)
        public async Task CreateInventoryForProductAsync(int productId)
        {
            var product = await _productRepo.GetProductByIdAsync(productId);
            if (product == null || product.Status != "ACTIVE")
                return;

            // Check if inventory already exists
            var existing = await _inventoryRepo.GetInventoriesByProductIdAsync(productId);
            if (existing.Any())
                return; // Already has inventory

            // Pick random location
            var random = new Random();
            var randomLocation = _locations[random.Next(_locations.Length)];

            var inventory = new Inventory
            {
                ProductId = product.Id,
                Location = randomLocation,
                CreatedAt = DateTime.UtcNow,
                Materials = product.BOMs.Select(bom => new InventoryMaterial
                {
                    MaterialName = bom.MaterialName,
                    AvailableQty = 0,
                    ThresholdQty = 0,
                    CreatedAt = DateTime.UtcNow
                }).ToList()
            };

            await _inventoryRepo.CreateInventoryAsync(inventory);
        }

        public async Task SyncInventoryMaterialsAsync(int productId)
        {
            var product = await _productRepo.GetProductByIdAsync(productId);
            if (product == null)
                throw new KeyNotFoundException($"Product {productId} not found");

            var inventories = await _inventoryRepo.GetInventoriesByProductIdAsync(productId);

            foreach (var inventory in inventories)
            {
                var bomMaterials = product.BOMs.Select(b => b.MaterialName).ToList();
                var existingMaterials = inventory.Materials.Select(m => m.MaterialName).ToList();

                // Add new materials
                var newMaterials = bomMaterials.Except(existingMaterials).ToList();
                foreach (var materialName in newMaterials)
                {
                    var newMaterial = new InventoryMaterial
                    {
                        InventoryId = inventory.InventoryId,
                        MaterialName = materialName,
                        AvailableQty = 0,  // ✅ Start at 0
                        ThresholdQty = 0,  // ✅ Start at 0
                        CreatedAt = DateTime.UtcNow
                    };

                    await _inventoryRepo.CreateMaterialAsync(newMaterial);
                }

                // Remove old materials
                var removedMaterials = existingMaterials.Except(bomMaterials).ToList();
                foreach (var materialName in removedMaterials)
                {
                    var material = inventory.Materials.FirstOrDefault(m => m.MaterialName == materialName);
                    if (material != null)
                    {
                        await _inventoryRepo.DeleteMaterialAsync(material.Id);
                    }
                }
            }
        }
    }
}
