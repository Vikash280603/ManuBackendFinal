// This is the INTERFACE for Product business logic
// The Service Layer sits between the Controller and the Repository.
// It contains business rules, validation, and DTO mapping.
//
// Controllers talk to Services.
// Services talk to Repositories.
// This keeps the architecture clean and maintainable.

using ManuBackend.DTOs;

namespace ManuBackend.Services
{
    public interface IProductService
    {
        // ============================================================
        // PRODUCT OPERATIONS
        // ============================================================

        // Get all products (optionally filtered by search term)
        // Returns a list of ProductDto objects
        Task<List<ProductDto>> GetAllProductsAsync(string? searchTerm = null);

        // Get a single product by ID
        // Returns null if not found
        Task<ProductDto?> GetProductByIdAsync(int id);

        // Create a new product
        // Accepts CreateProductDto (input model)
        // Returns ProductDto (output model)
        Task<ProductDto> CreateProductAsync(CreateProductDto dto);

        // Update an existing product by ID
        // Returns the updated ProductDto
        Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto dto);

        // Delete a product by ID
        // Returns true if deletion was successful
        Task<bool> DeleteProductAsync(int id);

        // ============================================================
        // BOM (Bill of Materials) OPERATIONS
        // ============================================================

        // Get all BOM items for a specific product
        Task<List<BOMDto>> GetBOMsByProductIdAsync(int productId);

        // Create a new BOM item for a product
        Task<BOMDto> CreateBOMAsync(int productId, CreateBOMDto dto);

        // Update an existing BOM item
        Task<BOMDto> UpdateBOMAsync(int bomId, UpdateBOMDto dto);

        // Delete a BOM item
        Task<bool> DeleteBOMAsync(int bomId);

        // Replace the entire BOM list for a product
        // Used when editing all BOM entries at once
        Task<List<BOMDto>> ReplaceBOMsAsync(
            int productId,
            List<CreateBOMDto> boms
        );
    }
}
