using ManuBackend.DTOs;

//using YourProject.Api.Modules.Inventory.DTOs;
namespace ManuBackend.Services
{
  

   
        public interface IInventoryService
        {
            // Read operations  
            Task<List<InventoryDto>> GetAllInventoriesAsync();
            Task<InventoryDto?> GetInventoryByIdAsync(int inventoryId);
            Task<List<InventoryDto>> GetInventoriesByProductIdAsync(int productId);

            // Material update operations  
            Task<InventoryMaterialDto> UpdateMaterialAsync(int materialId, UpdateInventoryMaterialDto dto);
            Task<InventoryMaterialDto> AdjustMaterialQuantityAsync(int materialId, int delta);

            // Special operations  
            Task<List<InventoryMaterialDto>> GetLowStockMaterialsAsync();

            // Admin operations  
            Task<int> GenerateInventoriesAsync();
            Task SyncInventoryMaterialsAsync(int productId);

            Task CreateInventoryForProductAsync(int productId);
        }
    }