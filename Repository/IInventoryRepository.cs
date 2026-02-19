// Interface for Inventory repository operations  

using ManuBackend.Models;
//using YourProject.Api.Modules.Inventory.Models;


namespace ManuBackend.Repository
{
    
        public interface IInventoryRepository
        {
            // Inventory operations  
            Task<List<Inventory>> GetAllInventoriesAsync();
            Task<Inventory?> GetInventoryByIdAsync(int inventoryId);
            Task<List<Inventory>> GetInventoriesByProductIdAsync(int productId);
            Task<Inventory> CreateInventoryAsync(Inventory inventory);

            // Material operations  
            Task<InventoryMaterial?> GetMaterialByIdAsync(int materialId);
            Task<InventoryMaterial> CreateMaterialAsync(InventoryMaterial material);
            Task<InventoryMaterial> UpdateMaterialAsync(InventoryMaterial material);
            Task<bool> DeleteMaterialAsync(int materialId);

            // Special queries  
            Task<List<InventoryMaterial>> GetLowStockMaterialsAsync();
        }
    }