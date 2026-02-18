// Interface for Inventory repository operations  

using ManuBackend.Models;
using ManuBackend.Models;

namespace ManuBackend.Repository
{
    public interface IInventoryRepository
    {
        // -------------------- INVENTORY OPERATIONS --------------------  

        Task<List<Inventory>> GetAllInventoriesAsync();
        Task<Inventory?> GetInventoryByIdAsync(int inventoryId);
        Task<List<Inventory>> GetInventoriesByProductIdAsync(int productId);
        Task<Inventory> CreateInventoryAsync(Inventory inventory);
        Task<Inventory> UpdateInventoryAsync(Inventory inventory);
        Task<bool> DeleteInventoryAsync(int inventoryId);

        // -------------------- MATERIAL OPERATIONS --------------------  

        Task<List<InventoryMaterial>> GetMaterialsByInventoryIdAsync(int inventoryId);
        Task<InventoryMaterial?> GetMaterialByIdAsync(int materialId);
        Task<InventoryMaterial> CreateMaterialAsync(InventoryMaterial material);
        Task<InventoryMaterial> UpdateMaterialAsync(InventoryMaterial material);
        Task<bool> DeleteMaterialAsync(int materialId);

        // Get all low stock materials across all inventories  
        Task<List<InventoryMaterial>> GetLowStockMaterialsAsync();
    }
}