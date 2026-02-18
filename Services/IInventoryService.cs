using ManuBackend.DTOs;

namespace ManuBackend.Services
{
    public interface IInventoryService
    {

        // -------------------- INVENTORY OPERATIONS --------------------  
        Task<List<InventoryDto>> GetAllInventoriesAsync();
        Task<InventoryDto?> GetInventoryByIdAsync(int inventoryId);
        Task<List<InventoryDto>> GetInventoriestByProductIdAsync(int productId);
        Task<InventoryDto> CreateInventoryAsync(CreateInventoryDto productDto);
        Task<InventoryDto> UpdateInventoryAsync(InventoryDto inventoryDto);
        Task<bool> DeleteInventoryAsync(int inventoryId);

        // -------------------- MATERIAL OPERATIONS --------------------  
        Task<List<InventoryMaterialsDto>> GetAllMaterialsByInventoryIdAsync(int inventoryId);
        Task<InventoryMaterialsDto> CreateMaterialAsync(int inventoryId, CreateInventoryMaterialDto dto);
        Task<InventoryMaterialsDto> UpdateMaterialAsync(int materialId, UpdateInventoryMaterialDto dto);
        Task<bool> DeleteMaterialAsync(int materialId);

        // -------------------- SPECIAL OPERATIONS --------------------  

        // Get all low stock materials for alerts  
        Task<List<InventoryMaterialsDto>> GetLowStockMaterialsAsync();

        // Adjust material quantity (for +/- buttons)  
        Task<InventoryMaterialsDto> AdjustMaterialQuantityAsync(int materialId, int adjustment);

    }
}
