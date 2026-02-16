// Interface for Product business logic  
// The service layer sits between Controller and Repository  

using ManuBackend.DTOs;


namespace ManuBackend.Services
{
    public interface IProductService
    {
        // -------------------- PRODUCT OPERATIONS --------------------  

        Task<List<ProductDto>> GetAllProductsAsync(string? searchTerm = null);
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<ProductDto> CreateProductAsync(CreateProductDto dto);
        Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto dto);
        Task<bool> DeleteProductAsync(int id);

        // -------------------- BOM OPERATIONS --------------------  

        Task<List<BOMDto>> GetBOMsByProductIdAsync(int productId);
        Task<BOMDto> CreateBOMAsync(int productId, CreateBOMDto dto);
        Task<BOMDto> UpdateBOMAsync(int bomId, UpdateBOMDto dto);
        Task<bool> DeleteBOMAsync(int bomId);

        // Replace entire BOM list for a product (used in EditBOM page)  
        Task<List<BOMDto>> ReplaceBOMsAsync(int productId, List<CreateBOMDto> boms);
    }
}