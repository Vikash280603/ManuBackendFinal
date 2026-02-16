// This is the IMPLEMENTATION of IProductRepository
// It contains the actual database logic using Entity Framework Core
// This class communicates directly with the database via AppDbContext

using ManuBackend.Data;
using ManuBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ManuBackend.Repository
{
    public class ProductRepository : IProductRepository
    {
        // AppDbContext represents the database session
        private readonly AppDbContext _context;

        // Constructor - injects the database context
        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // PRODUCT OPERATIONS
        // ============================================================

        // Get all products (optionally filtered by search term)
        public async Task<List<Product>> GetAllProductsAsync(string? searchTerm = null)
        {
            // Start building query
            var query = _context.Products
                .Include(p => p.BOMs) // Include related BOM items
                .AsQueryable();

            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p =>
                    p.Name.ToLower().Contains(searchTerm.ToLower()));
            }

            // Order by newest first
            return await query
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        // Get a single product by ID (including its BOM items)
        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.BOMs) // Eager load BOM entries
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // Create a new product
        public async Task<Product> CreateProductAsync(Product product)
        {
            _context.Products.Add(product);   // Track new entity
            await _context.SaveChangesAsync(); // Execute INSERT
            return product;
        }

        // Update an existing product
        public async Task<Product> UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);  // Mark as modified
            await _context.SaveChangesAsync();  // Execute UPDATE
            return product;
        }

        // Delete a product by ID
        // Related BOMs will be deleted automatically if cascade delete is configured
        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return false;

            _context.Products.Remove(product);  // Mark for deletion
            await _context.SaveChangesAsync();  // Execute DELETE
            return true;
        }

        // ============================================================
        // BOM (Bill of Materials) OPERATIONS
        // ============================================================

        // Get all BOM entries for a specific product
        public async Task<List<BOM>> GetBOMsByProductIdAsync(int productId)
        {
            return await _context.BOMs
                .Where(b => b.ProductId == productId)
                .OrderBy(b => b.BOMID)
                .ToListAsync();
        }

        // Get a single BOM entry by its ID
        public async Task<BOM?> GetBOMByIdAsync(int bomId)
        {
            return await _context.BOMs.FindAsync(bomId);
        }

        // Create a new BOM entry
        public async Task<BOM> CreateBOMAsync(BOM bom)
        {
            _context.BOMs.Add(bom);
            await _context.SaveChangesAsync();
            return bom;
        }

        // Update an existing BOM entry
        public async Task<BOM> UpdateBOMAsync(BOM bom)
        {
            _context.BOMs.Update(bom);
            await _context.SaveChangesAsync();
            return bom;
        }

        // Delete a single BOM entry
        public async Task<bool> DeleteBOMAsync(int bomId)
        {
            var bom = await _context.BOMs.FindAsync(bomId);
            if (bom == null)
                return false;

            _context.BOMs.Remove(bom);
            await _context.SaveChangesAsync();
            return true;
        }

        // Delete all BOM entries for a specific product
        // Used when replacing an entire BOM list
        public async Task DeleteAllBOMsForProductAsync(int productId)
        {
            var boms = await _context.BOMs
                .Where(b => b.ProductId == productId)
                .ToListAsync();

            _context.BOMs.RemoveRange(boms);
            await _context.SaveChangesAsync();
        }
    }
}
