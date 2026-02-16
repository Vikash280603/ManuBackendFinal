// Interface defining all database operations for Products and BOMs  

using ManuBackend.Models;


namespace ManuBackend.Repository
{
    public interface IProductRepository
    {
        // -------------------- PRODUCT OPERATIONS --------------------  

        // Get all products (with optional search filter)  
        Task<List<Product>> GetAllProductsAsync(string? searchTerm = null);

        // Get single product by ID (includes BOM items)  
        Task<Product?> GetProductByIdAsync(int id);

        // Create a new product  
        Task<Product> CreateProductAsync(Product product);

        // Update an existing product  
        Task<Product> UpdateProductAsync(Product product);

        // Delete a product (also deletes its BOMs via cascade)  
        Task<bool> DeleteProductAsync(int id);

        // -------------------- BOM OPERATIONS --------------------  

        // Get all BOM items for a specific product  
        Task<List<BOM>> GetBOMsByProductIdAsync(int productId);

        // Get single BOM item by ID  
        Task<BOM?> GetBOMByIdAsync(int bomId);

        // Create a new BOM entry  
        Task<BOM> CreateBOMAsync(BOM bom);

        // Update an existing BOM entry  
        Task<BOM> UpdateBOMAsync(BOM bom);

        // Delete a BOM entry  
        Task<bool> DeleteBOMAsync(int bomId);

        // Delete all BOMs for a product (used when replacing entire BOM list)  
        Task DeleteAllBOMsForProductAsync(int productId);
    }
}