// Implementation of IInventoryService

using ManuBackend.DTOs;
using ManuBackend.Models;
using ManuBackend.Repository;

namespace ManuBackend.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryService _inventoryService;

        public InventoryService(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        // -------------------- INVENTORY OPERATIONS --------------------

        public async Task<List<InventoryDto>> GetAllInventoriesAsync()
        {
            var inventories = await _inventoryService.GetAllInventoriesAsync();
            return inventories.Select(i => new InventoryDto
            {
                InventoryId = i.InventoryId,
                ProductId = i.ProductId,
                Location = i.Location,
                CreatedAt = i.CreatedAt,
                Materials = i.Materials.Select(m => new InventoryMaterialsDto
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
            var inventory = await _inventoryService.GetInventoryByIdAsync(inventoryId);
            if (inventory == null) return null;
            return new InventoryDto
            {
                InventoryId = inventory.InventoryId,
                ProductId = inventory.ProductId,
                Location = inventory.Location,
                CreatedAt = inventory.CreatedAt,
                Materials = inventory.Materials.Select(m => new InventoryMaterialsDto
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

        public async Task<List<InventoryDto>> GetInventoriestByProductIdAsync(int productId)
        {
            var inventories = await _inventoryService.GetInventoriestByProductIdAsync(productId);
            return inventories.Select(i => new InventoryDto
            {
                InventoryId = i.InventoryId,
                ProductId = i.ProductId,
                Location = i.Location,
                CreatedAt = i.CreatedAt,
                Materials = i.Materials.Select(m => new InventoryMaterialsDto
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

        public async Task<InventoryDto> CreateInventoryAsync(CreateInventoryDto dto)
        {
            // Validate location
            var allowedLocations = new[] { "Chennai", "Coimbatore", "Bangalore", "Hyderabad" };
            if (!allowedLocations.Contains(dto.Location))
            {
                throw new InvalidOperationException("Invalid location");
            }

            var inventory = new Inventory
            {
                ProductId = dto.ProductId,
                Location = dto.Location,
                CreatedAt = DateTime.UtcNow
            };

            // If materials provided, create them too
            if (dto.Materials != null && dto.Materials.Any())
            {
                inventory.Materials = dto.Materials.Select(m => new InventoryMaterial
                {
                    MaterialName = m.MaterialName,
                    AvailableQty = m.AvailableQty,
                    ThresholdQty = m.ThresholdQty,
                    CreatedAt = DateTime.UtcNow
                }).ToList();
            }

            var saved = await _inventoryService.CreateInventoryAsync(inventory);

            return new InventoryDto
            {
                InventoryId = saved.InventoryId,
                ProductId = saved.ProductId,
                Location = saved.Location,
                CreatedAt = saved.CreatedAt,
                Materials = saved.Materials.Select(m => new InventoryMaterialsDto
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

        public async Task<InventoryDto> UpdateInventoryAsync(int inventoryId, UpdateInventoryDto dto)
        {
            var inventory = await _inventoryService.GetInventoryByIdAsync(inventoryId);
            if (inventory == null)
                throw new KeyNotFoundException($"Inventory {inventoryId} not found");

            if (!string.IsNullOrWhiteSpace(dto.Location))
                inventory.Location = dto.Location;

            var updated = await _inventoryService.UpdateInventoryAsync(inventory);

            return new InventoryDto
            {
                InventoryId = updated.InventoryId,
                ProductId = updated.ProductId,
                Location = updated.Location,
                CreatedAt = updated.CreatedAt,
                Materials = updated.Materials.Select(m => new InventoryMaterialsDto
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

        public async Task<bool> DeleteInventoryAsync(int inventoryId)
        {
            return await _inventoryService.DeleteInventoryAsync(inventoryId);
        }

        // -------------------- MATERIAL OPERATIONS --------------------

        public async Task<List<InventoryMaterialsDto>> GetMaterialsByInventoryIdAsync(int inventoryId)
        {
            var materials = await _inventoryService.GetMaterialsByInventoryIdAsync(inventoryId);

            return materials.Select(m => new InventoryMaterialsDto
            {
                Id = m.Id,
                InventoryId = m.InventoryId,
                MaterialName = m.MaterialName,
                AvailableQty = m.AvailableQty,
                ThresholdQty = m.ThresholdQty,
                CreatedAt = m.CreatedAt
            }).ToList();
        }

        public async Task<InventoryMaterialsDto> CreateMaterialAsync(int inventoryId, CreateInventoryMaterialDto dto)
        {
            // Verify inventory exists
            var inventory = await _inventoryService.GetInventoryByIdAsync(inventoryId);
            if (inventory == null)
                throw new KeyNotFoundException($"Inventory {inventoryId} not found");

            var material = new InventoryMaterial
            {
                InventoryId = inventoryId,
                MaterialName = dto.MaterialName,
                AvailableQty = dto.AvailableQty,
                ThresholdQty = dto.ThresholdQty,
                CreatedAt = DateTime.UtcNow
            };

            var saved = await _inventoryService.CreateMaterialAsync(material);

            return new InventoryMaterialsDto
            {
                Id = saved.Id,
                InventoryId = saved.InventoryId,
                MaterialName = saved.MaterialName,
                AvailableQty = saved.AvailableQty,
                ThresholdQty = saved.ThresholdQty,
                CreatedAt = saved.CreatedAt
            };
        }

        public async Task<InventoryMaterialsDto> UpdateMaterialAsync(int materialId, UpdateInventoryMaterialDto dto)
        {
            var material = await _inventoryService.GetMaterialByIdAsync(materialId);
            if (material == null)
                throw new KeyNotFoundException($"Material {materialId} not found");

            if (!string.IsNullOrWhiteSpace(dto.MaterialName))
                material.MaterialName = dto.MaterialName;

            if (dto.AvailableQty.HasValue)
                material.AvailableQty = dto.AvailableQty.Value;

            if (dto.ThresholdQty.HasValue)
                material.ThresholdQty = dto.ThresholdQty.Value;

            var updated = await _inventoryService.UpdateMaterialAsync(material);

            return new InventoryMaterialsDto
            {
                Id = updated.Id,
                InventoryId = updated.InventoryId,
                MaterialName = updated.MaterialName,
                AvailableQty = updated.AvailableQty,
                ThresholdQty = updated.ThresholdQty,
                CreatedAt = updated.CreatedAt
            };
        }

        public async Task<bool> DeleteMaterialAsync(int materialId)
        {
            return await _inventoryService.DeleteMaterialAsync(materialId);
        }

        // -------------------- SPECIAL OPERATIONS --------------------

        public async Task<List<InventoryMaterialsDto>> GetLowStockMaterialsAsync()
        {
            var materials = await _inventoryService.GetLowStockMaterialsAsync();

            return materials.Select(m => new InventoryMaterialsDto
            {
                Id = m.Id,
                InventoryId = m.InventoryId,
                MaterialName = m.MaterialName,
                AvailableQty = m.AvailableQty,
                ThresholdQty = m.ThresholdQty,
                CreatedAt = m.CreatedAt
            }).ToList();
        }

        // Adjust quantity by delta (+1 or -1 from frontend)
        public async Task<InventoryMaterialsDto> AdjustMaterialQuantityAsync(int materialId, int delta)
        {
            var material = await _inventoryService.GetMaterialByIdAsync(materialId);
            if (material == null)
                throw new KeyNotFoundException($"Material {materialId} not found");

            // Adjust quantity, ensure it doesn't go below 0
            material.AvailableQty = Math.Max(0, material.AvailableQty + delta);

            var updated = await _inventoryService.UpdateMaterialAsync(material);

            return new InventoryMaterialsDto
            {
                Id = updated.Id,
                InventoryId = updated.InventoryId,
                MaterialName = updated.MaterialName,
                AvailableQty = updated.AvailableQty,
                ThresholdQty = updated.ThresholdQty,
                CreatedAt = updated.CreatedAt
            };
        }

        public Task<InventoryDto> UpdateInventoryAsync(InventoryDto inventoryDto)
        {
            throw new NotImplementedException();
        }

        public Task<List<InventoryMaterialsDto>> GetAllMaterialsByInventoryIdAsync(int inventoryId)
        {
            throw new NotImplementedException();
        }
    };
}
