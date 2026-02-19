
using ManuBackend.Data;
using ManuBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ManuBackend.Repository
{


   
        public class InventoryRepository : IInventoryRepository
        {
            private readonly AppDbContext _context;

            public InventoryRepository(AppDbContext context)
            {
                _context = context;
            }

            // -------------------- INVENTORY OPERATIONS --------------------  

            public async Task<List<Inventory>> GetAllInventoriesAsync()
            {
                return await _context.Inventories
                    .Include(i => i.Materials)
                    .OrderByDescending(i => i.CreatedAt)
                    .ToListAsync();
            }

            public async Task<Inventory?> GetInventoryByIdAsync(int inventoryId)
            {
                return await _context.Inventories
                    .Include(i => i.Materials)
                    .FirstOrDefaultAsync(i => i.InventoryId == inventoryId);
            }

            public async Task<List<Inventory>> GetInventoriesByProductIdAsync(int productId)
            {
                return await _context.Inventories
                    .Include(i => i.Materials)
                    .Where(i => i.ProductId == productId)
                    .ToListAsync();
            }

            public async Task<Inventory> CreateInventoryAsync(Inventory inventory)
            {
                _context.Inventories.Add(inventory);
                await _context.SaveChangesAsync();
                return inventory;
            }

            // -------------------- MATERIAL OPERATIONS --------------------  

            public async Task<InventoryMaterial?> GetMaterialByIdAsync(int materialId)
            {
                return await _context.InventoryMaterials.FindAsync(materialId);
            }

            public async Task<InventoryMaterial> CreateMaterialAsync(InventoryMaterial material)
            {
                _context.InventoryMaterials.Add(material);
                await _context.SaveChangesAsync();
                return material;
            }

            public async Task<InventoryMaterial> UpdateMaterialAsync(InventoryMaterial material)
            {
                _context.InventoryMaterials.Update(material);
                await _context.SaveChangesAsync();
                return material;
            }

            public async Task<bool> DeleteMaterialAsync(int materialId)
            {
                var material = await _context.InventoryMaterials.FindAsync(materialId);
                if (material == null) return false;

                _context.InventoryMaterials.Remove(material);
                await _context.SaveChangesAsync();
                return true;
            }

            public async Task<List<InventoryMaterial>> GetLowStockMaterialsAsync()
            {
                return await _context.InventoryMaterials
                    .Include(m => m.Inventory)
                    .Where(m => m.AvailableQty < m.ThresholdQty)
                    .OrderBy(m => m.AvailableQty)
                    .ToListAsync();
            }
        }
    }