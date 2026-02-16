// Interface defining all database operations related to Products and BOMs
// This acts as a contract: any class implementing IProductRepository
// MUST provide concrete implementations for all these methods.
// It keeps the data layer clean, testable, and replaceable.

using ManuBackend.Models;

namespace ManuBackend.Repository
{
    public interface IProductRepository
    {
        // ============================================================
        // PRODUCT OPERATIONS
        // ============================================================

        // Get all products
        // Optional searchTerm allows filtering by name or other fields
        // Returns a list of Product objects
        Task<List<Product>> GetAllProductsAsync(string? searchTerm = null);

        // Get a single product by its ID
        // Includes related BOM items (via navigation property)
        // Returns null if product does not exist
        Task<Product?> GetProductByIdAsync(int id);

        // Create and save a new product
        // Returns the created Product object
        Task<Product> CreateProductAsync(Product product);

        // Update an existing product
        // Returns the updated Product object
        Task<Product> UpdateProductAsync(Product product);

        // Delete a product by ID
        // Cascade delete will also remove related BOM entries
        // Returns true if deletion was successful
        Task<bool> DeleteProductAsync(int id);

        // ============================================================
        // BOM (Bill of Materials) OPERATIONS
        // ============================================================

        // Get all BOM entries for a specific product
        Task<List<BOM>> GetBOMsByProductIdAsync(int productId);

        // Get a single BOM entry by its ID
        // Returns null if not found
        Task<BOM?> GetBOMByIdAsync(int bomId);

        // Create and save a new BOM entry
        Task<BOM> CreateBOMAsync(BOM bom);

        // Update an existing BOM entry
        Task<BOM> UpdateBOMAsync(BOM bom);

        // Delete a single BOM entry by ID
        // Returns true if deletion was successful
        Task<bool> DeleteBOMAsync(int bomId);

        // Delete all BOM entries for a specific product
        // Typically used when replacing the entire BOM list
        Task DeleteAllBOMsForProductAsync(int productId);
    }
}
