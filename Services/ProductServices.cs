// Implementation of IProductService  
// Contains business logic, validation, and DTO mapping  

using ManuBackend.DTOs;
using ManuBackend.Models;
using ManuBackend.Repository;


namespace ManuBackend.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;

        public ProductService(IProductRepository repo)
        {
            _repo = repo;
        }

        // -------------------- PRODUCT OPERATIONS --------------------  

        public async Task<List<ProductDto>> GetAllProductsAsync(string? searchTerm = null)
        {
            var products = await _repo.GetAllProductsAsync(searchTerm);

            // Map Product entities to ProductDto  
            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Category = p.Category,
                Status = p.Status,
                CreatedAt = p.CreatedAt,
                BOMs = p.BOMs.Select(b => new BOMDto
                {
                    BOMID = b.BOMID,
                    ProductId = b.ProductId,
                    MaterialName = b.MaterialName,
                    Quantity = b.Quantity,
                    CreatedAt = b.CreatedAt
                }).ToList()
            }).ToList();
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await _repo.GetProductByIdAsync(id);
            if (product == null) return null;

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Category = product.Category,
                Status = product.Status,
                CreatedAt = product.CreatedAt,
                BOMs = product.BOMs.Select(b => new BOMDto
                {
                    BOMID = b.BOMID,
                    ProductId = b.ProductId,
                    MaterialName = b.MaterialName,
                    Quantity = b.Quantity,
                    CreatedAt = b.CreatedAt
                }).ToList()
            };
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
        {
            // Validate allowed categories  
            var allowedCategories = new[] { "Mechanical", "Electrical", "Packaging", "Construction", "Tools" };
            if (!allowedCategories.Contains(dto.Category))
            {
                throw new InvalidOperationException("Invalid category selected");
            }

            // Validate allowed statuses  
            var allowedStatuses = new[] { "ACTIVE", "DISCONTINUED" };
            if (!allowedStatuses.Contains(dto.Status))
            {
                throw new InvalidOperationException("Invalid status selected");
            }

            // Create product entity  
            var product = new Product
            {
                Name = dto.Name,
                Category = dto.Category,
                Status = dto.Status,
                CreatedAt = DateTime.UtcNow
            };

            var savedProduct = await _repo.CreateProductAsync(product);

            return new ProductDto
            {
                Id = savedProduct.Id,
                Name = savedProduct.Name,
                Category = savedProduct.Category,
                Status = savedProduct.Status,
                CreatedAt = savedProduct.CreatedAt,
                BOMs = new List<BOMDto>()
            };
        }

        public async Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto dto)
        {
            var product = await _repo.GetProductByIdAsync(id);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {id} not found");

            // Update only provided fields  
            if (!string.IsNullOrWhiteSpace(dto.Name))
                product.Name = dto.Name;

            if (!string.IsNullOrWhiteSpace(dto.Category))
                product.Category = dto.Category;

            if (!string.IsNullOrWhiteSpace(dto.Status))
                product.Status = dto.Status;

            var updated = await _repo.UpdateProductAsync(product);

            return new ProductDto
            {
                Id = updated.Id,
                Name = updated.Name,
                Category = updated.Category,
                Status = updated.Status,
                CreatedAt = updated.CreatedAt,
                BOMs = updated.BOMs.Select(b => new BOMDto
                {
                    BOMID = b.BOMID,
                    ProductId = b.ProductId,
                    MaterialName = b.MaterialName,
                    Quantity = b.Quantity,
                    CreatedAt = b.CreatedAt
                }).ToList()
            };
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            return await _repo.DeleteProductAsync(id);
        }

        // -------------------- BOM OPERATIONS --------------------  

        public async Task<List<BOMDto>> GetBOMsByProductIdAsync(int productId)
        {
            var boms = await _repo.GetBOMsByProductIdAsync(productId);

            return boms.Select(b => new BOMDto
            {
                BOMID = b.BOMID,
                ProductId = b.ProductId,
                MaterialName = b.MaterialName,
                Quantity = b.Quantity,
                CreatedAt = b.CreatedAt
            }).ToList();
        }

        public async Task<BOMDto> CreateBOMAsync(int productId, CreateBOMDto dto)
        {
            // Verify product exists  
            var product = await _repo.GetProductByIdAsync(productId);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {productId} not found");

            var bom = new BOM
            {
                ProductId = productId,
                MaterialName = dto.MaterialName,
                Quantity = dto.Quantity,
                CreatedAt = DateTime.UtcNow
            };

            var saved = await _repo.CreateBOMAsync(bom);

            return new BOMDto
            {
                BOMID = saved.BOMID,
                ProductId = saved.ProductId,
                MaterialName = saved.MaterialName,
                Quantity = saved.Quantity,
                CreatedAt = saved.CreatedAt
            };
        }

        public async Task<BOMDto> UpdateBOMAsync(int bomId, UpdateBOMDto dto)
        {
            var bom = await _repo.GetBOMByIdAsync(bomId);
            if (bom == null)
                throw new KeyNotFoundException($"BOM with ID {bomId} not found");

            // Update only provided fields  
            if (!string.IsNullOrWhiteSpace(dto.MaterialName))
                bom.MaterialName = dto.MaterialName;

            if (dto.Quantity.HasValue)
                bom.Quantity = dto.Quantity.Value;

            var updated = await _repo.UpdateBOMAsync(bom);

            return new BOMDto
            {
                BOMID = updated.BOMID,
                ProductId = updated.ProductId,
                MaterialName = updated.MaterialName,
                Quantity = updated.Quantity,
                CreatedAt = updated.CreatedAt
            };
        }

        public async Task<bool> DeleteBOMAsync(int bomId)
        {
            return await _repo.DeleteBOMAsync(bomId);
        }

        // Replace entire BOM list (used in EditBOM page)  
        public async Task<List<BOMDto>> ReplaceBOMsAsync(int productId, List<CreateBOMDto> boms)
        {
            // Verify product exists  
            var product = await _repo.GetProductByIdAsync(productId);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {productId} not found");

            // Delete all existing BOMs for this product  
            await _repo.DeleteAllBOMsForProductAsync(productId);

            // Create new BOMs  
            var createdBOMs = new List<BOM>();
            foreach (var dto in boms)
            {
                var bom = new BOM
                {
                    ProductId = productId,
                    MaterialName = dto.MaterialName,
                    Quantity = dto.Quantity,
                    CreatedAt = DateTime.UtcNow
                };

                var saved = await _repo.CreateBOMAsync(bom);
                createdBOMs.Add(saved);
            }

            return createdBOMs.Select(b => new BOMDto
            {
                BOMID = b.BOMID,
                ProductId = b.ProductId,
                MaterialName = b.MaterialName,
                Quantity = b.Quantity,
                CreatedAt = b.CreatedAt
            }).ToList();
        }
    }
}