// Implementation of IProductRepository  
// Contains actual database queries using EF Core  

using ManuBackend.Data;
using ManuBackend.Models;
using ManuBackend.Repository;
using Microsoft.EntityFrameworkCore;
//using YourProject.Api.Data;
//using YourProject.Api.Modules.Products.Models;    

namespace ManuBackend.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        // -------------------- PRODUCT OPERATIONS --------------------  

        public async Task<List<Product>> GetAllProductsAsync(string? searchTerm = null)
        {
            var query = _context.Products
                .Include(p => p.BOMs) // Include BOM items  
                .AsQueryable();

            // If search term provided, filter by product name  
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.Name.ToLower().Contains(searchTerm.ToLower()));
            }

            return await query
                .OrderByDescending(p => p.CreatedAt) // Newest first  
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.BOMs) // Include BOM items  
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        // -------------------- BOM OPERATIONS --------------------  

        public async Task<List<BOM>> GetBOMsByProductIdAsync(int productId)
        {
            return await _context.BOMs
                .Where(b => b.ProductId == productId)
                .OrderBy(b => b.BOMID)
                .ToListAsync();
        }

        public async Task<BOM?> GetBOMByIdAsync(int bomId)
        {
            return await _context.BOMs.FindAsync(bomId);
        }

        public async Task<BOM> CreateBOMAsync(BOM bom)
        {
            _context.BOMs.Add(bom);
            await _context.SaveChangesAsync();
            return bom;
        }

        public async Task<BOM> UpdateBOMAsync(BOM bom)
        {
            _context.BOMs.Update(bom);
            await _context.SaveChangesAsync();
            return bom;
        }

        public async Task<bool> DeleteBOMAsync(int bomId)
        {
            var bom = await _context.BOMs.FindAsync(bomId);
            if (bom == null) return false;

            _context.BOMs.Remove(bom);
            await _context.SaveChangesAsync();
            return true;
        }

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