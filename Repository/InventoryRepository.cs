using ManuBackend.Data;
using ManuBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ManuBackend.Repository
{
    // Repository = Database access layer
    // ONLY talks to DbContext
    // NO business logic here
    public class InventoryRepository : IInventoryRepository
    {
        // EF Core DbContext
        private readonly AppDbContext _context;

        // Constructor Injection
        public InventoryRepository(AppDbContext context)
        {
            _context = context;
        }

        // -------------------- INVENTORY OPERATIONS --------------------

        // Get ALL inventory records
        public async Task<List<Inventory>> GetAllInventoriesAsync()
        {
            return await _context.Inventories

                // Include Materials table (Eager Loading)
                .Include(i => i.Materials)

                // Latest inventory first
                .OrderByDescending(i => i.CreatedAt)

                // Execute query and return list
                .ToListAsync();
        }

        // Get ONE inventory by InventoryId
        public async Task<Inventory?> GetInventoryByIdAsync(int inventoryId)
        {
            return await _context.Inventories
                .Include(i => i.Materials)
                .FirstOrDefaultAsync(i => i.InventoryId == inventoryId);
        }

        // Get inventories for a specific product
        public async Task<List<Inventory>> GetInventoriesByProductIdAsync(int productId)
        {
            return await _context.Inventories
                .Include(i => i.Materials)
                .Where(i => i.ProductId == productId)
                .ToListAsync();
        }

        // Create new inventory record
        public async Task<Inventory> CreateInventoryAsync(Inventory inventory)
        {
            _context.Inventories.Add(inventory);
            await _context.SaveChangesAsync();
            return inventory;
        }

        // -------------------- MATERIAL OPERATIONS --------------------

        // Get material by MaterialId
        public async Task<InventoryMaterial?> GetMaterialByIdAsync(int materialId)
        {
            // FindAsync uses Primary Key
            return await _context.InventoryMaterials.FindAsync(materialId);
        }

        // Create new inventory material
        public async Task<InventoryMaterial> CreateMaterialAsync(InventoryMaterial material)
        {
            _context.InventoryMaterials.Add(material);
            await _context.SaveChangesAsync();
            return material;
        }

        // Update material quantity or threshold
        public async Task<InventoryMaterial> UpdateMaterialAsync(InventoryMaterial material)
        {
            _context.InventoryMaterials.Update(material);
            await _context.SaveChangesAsync();
            return material;
        }

        // Delete material by ID
        public async Task<bool> DeleteMaterialAsync(int materialId)
        {
            var material = await _context.InventoryMaterials.FindAsync(materialId);

            if (material == null)
                return false;

            _context.InventoryMaterials.Remove(material);
            await _context.SaveChangesAsync();
            return true;
        }

        // Get materials where stock is LOW
        public async Task<List<InventoryMaterial>> GetLowStockMaterialsAsync()
        {
            return await _context.InventoryMaterials

                // Include Inventory details
                .Include(m => m.Inventory)

                // Business condition for low stock
                .Where(m => m.AvailableQty < m.ThresholdQty)

                // Lowest stock first
                .OrderBy(m => m.AvailableQty)

                .ToListAsync();
        }
    }
}