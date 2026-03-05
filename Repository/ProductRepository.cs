// =============================================================
// PRODUCT REPOSITORY
// =============================================================
//
// This class IMPLEMENTS IProductRepository
// It contains ONLY database logic
//
// Responsibilities:
// - Talk directly to the database
// - Use Entity Framework Core (DbContext)
// - Perform CRUD operations
//
// IMPORTANT INTERVIEW POINT:
// ❌ No business logic here
// ❌ No DTOs here
// ✅ Only Entities + EF Core
// =============================================================

using ManuBackend.Data;
using ManuBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ManuBackend.Repository
{
    public class ProductRepository : IProductRepository
    {
        // AppDbContext represents a connection/session with the database
        // EF Core uses this to track and save entities
        private readonly AppDbContext _context;

        // -------------------- CONSTRUCTOR --------------------
        // DbContext is injected via Dependency Injection
        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // PRODUCT OPERATIONS
        // =====================================================

        // -------------------- GET ALL PRODUCTS --------------------
        // Optional searchTerm for filtering by product name
        public async Task<List<Product>> GetAllProductsAsync(string? searchTerm = null)
        {
            // Start building the EF Core query
            // AsQueryable allows conditional query building
            var query = _context.Products
                .Include(p => p.BOMs) // Eager loading: include related BOMs
                .AsQueryable();

            // Apply search filter ONLY if searchTerm is provided
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p =>
                    p.Name.ToLower().Contains(searchTerm.ToLower()));
            }

            // Order products by newest first and execute query
            return await query
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(); // Executes SQL SELECT
        }

        // -------------------- GET PRODUCT BY ID --------------------
        // Returns null if product does not exist
        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.BOMs) // Load related BOM records
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // -------------------- CREATE PRODUCT --------------------
        public async Task<Product> CreateProductAsync(Product product)
        {
            _context.Products.Add(product);   // Mark entity as Added
            await _context.SaveChangesAsync(); // Executes INSERT SQL
            return product;
        }

        // -------------------- UPDATE PRODUCT --------------------
        public async Task<Product> UpdateProductAsync(Product product)
        {
            _context.Products.Update(product); // Mark entity as Modified
            await _context.SaveChangesAsync(); // Executes UPDATE SQL
            return product;
        }

        // -------------------- DELETE PRODUCT --------------------
        // If cascade delete is configured, related BOMs are deleted automatically
        public async Task<bool> DeleteProductAsync(int id)
        {
            // Find product by primary key
            var product = await _context.Products.FindAsync(id); // default FindAsync searches only primary key!

            if (product == null)
                return false;

            _context.Products.Remove(product); // Mark entity as Deleted
            await _context.SaveChangesAsync(); // Executes DELETE SQL
            return true;
        }

        // =====================================================
        // BOM (Bill Of Materials) OPERATIONS
        // =====================================================

        // -------------------- GET BOMs BY PRODUCT ID --------------------
        public async Task<List<BOM>> GetBOMsByProductIdAsync(int productId)
        {
            return await _context.BOMs
                .Where(b => b.ProductId == productId) // Filter by product
                .OrderBy(b => b.BOMID)                // Consistent ordering
                .ToListAsync();                       // Execute SELECT
        }

        // -------------------- GET BOM BY ID --------------------
        public async Task<BOM?> GetBOMByIdAsync(int bomId)
        {
            // FindAsync uses primary key
            return await _context.BOMs.FindAsync(bomId);
        }

        // -------------------- CREATE BOM --------------------
        public async Task<BOM> CreateBOMAsync(BOM bom)
        {
            _context.BOMs.Add(bom);        // Track new BOM
            await _context.SaveChangesAsync(); // INSERT
            return bom;
        }

        // -------------------- UPDATE BOM --------------------
        public async Task<BOM> UpdateBOMAsync(BOM bom)
        {
            _context.BOMs.Update(bom);     // Mark as modified
            await _context.SaveChangesAsync(); // UPDATE
            return bom;
        }

        // -------------------- DELETE BOM --------------------
        public async Task<bool> DeleteBOMAsync(int bomId)
        {
            var bom = await _context.BOMs.FindAsync(bomId);

            if (bom == null)
                return false;

            _context.BOMs.Remove(bom);     // Mark for deletion
            await _context.SaveChangesAsync(); // DELETE
            return true;
        }

        // -------------------- DELETE ALL BOMs FOR A PRODUCT --------------------
        // Used when replacing an entire BOM list
        public async Task DeleteAllBOMsForProductAsync(int productId)
        {
            // Fetch all BOMs related to the product
            var boms = await _context.BOMs
                .Where(b => b.ProductId == productId)
                .ToListAsync();

            // Remove all in one operation
            _context.BOMs.RemoveRange(boms);
            await _context.SaveChangesAsync();
        }
    }
}