// =============================================================
// PRODUCT SERVICE
// =============================================================
//
// This class IMPLEMENTS IProductService
// It contains:
// - Business logic
// - Validation rules
// - DTO ↔ Entity mapping
//
// IMPORTANT:
// ❌ No database code here
// ✅ Database access happens via Repository
// =============================================================

using ManuBackend.DTOs;
using ManuBackend.Models;
using ManuBackend.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace ManuBackend.Services
{
    public class ProductService : IProductService
    {
        // Repository dependency (data access)
        private readonly IProductRepository _repo;

        // IServiceProvider is used to resolve services dynamically
        // This avoids circular dependency between ProductService & InventoryService
        private readonly IServiceProvider _serviceProvider;

        // -------------------- CONSTRUCTOR --------------------
        // Dependencies are injected by ASP.NET Core DI container
        public ProductService(IProductRepository repo, IServiceProvider serviceProvider)
        {
            _repo = repo;
            _serviceProvider = serviceProvider;
        }

        // =====================================================
        // PRODUCT OPERATIONS
        // =====================================================

        // -------------------- GET ALL PRODUCTS --------------------
        // Fetches products from repository
        // Applies optional search filter
        // Converts Entity → DTO
        public async Task<List<ProductDto>> GetAllProductsAsync(string? searchTerm = null)
        {
            // Repository returns List<Product>
            var products = await _repo.GetAllProductsAsync(searchTerm);

            // Map Product entity → ProductDto
            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Category = p.Category,
                Status = p.Status,
                CreatedAt = p.CreatedAt,

                // Nested mapping: BOM entity → BOMDto
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

        // -------------------- GET PRODUCT BY ID --------------------
        // Returns null if product not found
        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await _repo.GetProductByIdAsync(id);

            // Defensive check
            if (product == null) return null;

            // Map Entity → DTO
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

        // -------------------- CREATE PRODUCT --------------------
        // Also auto-creates inventory for the product
        public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
        {
            // ---------------- VALIDATION ----------------

            // Allowed categories whitelist
            var allowedCategories = new[]
            {
                "Mechanical", "Electrical", "Packaging", "Construction", "Tools"
            };

            if (!allowedCategories.Contains(dto.Category))
            {
                throw new InvalidOperationException("Invalid category selected");
            }

            // Allowed status values
            var allowedStatuses = new[] { "ACTIVE", "DISCONTINUED" };

            if (!allowedStatuses.Contains(dto.Status))
            {
                throw new InvalidOperationException("Invalid status selected");
            }

            // ---------------- ENTITY CREATION ----------------

            var product = new Product
            {
                Name = dto.Name,
                Category = dto.Category,
                Status = dto.Status,
                CreatedAt = DateTime.UtcNow
            };

            // Save product to database
            var savedProduct = await _repo.CreateProductAsync(product);

            // ---------------- AUTO INVENTORY CREATION ----------------
            // Inventory is created AFTER product creation
            // Inventory initially has NO materials (BOMs not added yet)

            try
            {
                // Create a new DI scope
                using (var scope = _serviceProvider.CreateScope())
                {
                    // Resolve InventoryService safely
                    var inventoryService = scope.ServiceProvider
                        .GetRequiredService<IInventoryService>();

                    await inventoryService
                        .CreateInventoryForProductAsync(savedProduct.Id);
                }
            }
            catch (Exception ex)
            {
                // Product creation should NOT fail due to inventory failure
                Console.WriteLine($"Failed to create inventory: {ex.Message}");
            }

            // Return DTO (API-friendly object)
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

        // -------------------- UPDATE PRODUCT --------------------
        public async Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto dto)
        {
            // Fetch existing product
            var product = await _repo.GetProductByIdAsync(id);

            if (product == null)
                throw new KeyNotFoundException($"Product with ID {id} not found");

            // Update ONLY fields provided by client
            if (!string.IsNullOrWhiteSpace(dto.Name))
                product.Name = dto.Name;

            if (!string.IsNullOrWhiteSpace(dto.Category))
                product.Category = dto.Category;

            if (!string.IsNullOrWhiteSpace(dto.Status))
                product.Status = dto.Status;

            var updated = await _repo.UpdateProductAsync(product);

            // Map updated entity → DTO
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

        // -------------------- DELETE PRODUCT --------------------
        public async Task<bool> DeleteProductAsync(int id)
        {
            return await _repo.DeleteProductAsync(id);
        }

        // =====================================================
        // BOM OPERATIONS
        // =====================================================

        // -------------------- GET BOMs BY PRODUCT --------------------
        public async Task<List<BOMDto>> GetBOMsByProductIdAsync(int productId)
        {
            var boms = await _repo.GetBOMsByProductIdAsync(productId);

            return boms.OrderBy(b=>b.MaterialName).Select(b => new BOMDto
            {
                BOMID = b.BOMID,
                ProductId = b.ProductId,
                MaterialName = b.MaterialName,
                Quantity = b.Quantity,
                CreatedAt = b.CreatedAt
            }).ToList();
        }

        // -------------------- CREATE BOM --------------------
        // Syncs inventory materials after BOM creation
        public async Task<BOMDto> CreateBOMAsync(int productId, CreateBOMDto dto)
        {
            // Ensure product exists
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

            // Sync inventory after BOM creation
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var inventoryService = scope.ServiceProvider
                        .GetRequiredService<IInventoryService>();

                    await inventoryService
                        .SyncInventoryMaterialsAsync(productId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to sync inventory materials: {ex.Message}");
            }

            return new BOMDto
            {
                BOMID = saved.BOMID,
                ProductId = saved.ProductId,
                MaterialName = saved.MaterialName,
                Quantity = saved.Quantity,
                CreatedAt = saved.CreatedAt
            };
        }

        // -------------------- UPDATE BOM --------------------
        public async Task<BOMDto> UpdateBOMAsync(int bomId, UpdateBOMDto dto)
        {
            var bom = await _repo.GetBOMByIdAsync(bomId);

            if (bom == null)
                throw new KeyNotFoundException($"BOM with ID {bomId} not found");

            // Update selectively
            if (!string.IsNullOrWhiteSpace(dto.MaterialName))
                bom.MaterialName = dto.MaterialName;

            if (dto.Quantity.HasValue)
                bom.Quantity = dto.Quantity.Value;

            var updated = await _repo.UpdateBOMAsync(bom);

            // Sync inventory after BOM update
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var inventoryService = scope.ServiceProvider
                        .GetRequiredService<IInventoryService>();

                    await inventoryService
                        .SyncInventoryMaterialsAsync(updated.ProductId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to sync inventory materials: {ex.Message}");
            }

            return new BOMDto
            {
                BOMID = updated.BOMID,
                ProductId = updated.ProductId,
                MaterialName = updated.MaterialName,
                Quantity = updated.Quantity,
                CreatedAt = updated.CreatedAt
            };
        }

        // -------------------- DELETE BOM --------------------
        public async Task<bool> DeleteBOMAsync(int bomId)
        {
            var bom = await _repo.GetBOMByIdAsync(bomId);
            if (bom == null) return false;

            var productId = bom.ProductId;
            var deleted = await _repo.DeleteBOMAsync(bomId);

            // Sync inventory after deletion
            if (deleted)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var inventoryService = scope.ServiceProvider
                            .GetRequiredService<IInventoryService>();

                        await inventoryService
                            .SyncInventoryMaterialsAsync(productId);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to sync inventory materials: {ex.Message}");
                }
            }

            return deleted;
        }

        // -------------------- REPLACE ALL BOMs --------------------
        // Deletes existing BOMs and inserts new ones
        public async Task<List<BOMDto>> ReplaceBOMsAsync(
            int productId,
            List<CreateBOMDto> boms)
        {
            // Ensure product exists
            var product = await _repo.GetProductByIdAsync(productId);

            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {productId} not found");
            }
            else if (product.Status != "ACTIVE")
            {
                throw new InvalidOperationException($"Product with ID {productId} is not active");
            }

            // Remove existing BOMs
            await _repo.DeleteAllBOMsForProductAsync(productId);

            var createdBOMs = new List<BOM>();

            // Insert new BOMs
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

            // Sync inventory after replacement
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var inventoryService = scope.ServiceProvider
                        .GetRequiredService<IInventoryService>();

                    await inventoryService
                        .SyncInventoryMaterialsAsync(productId);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to sync inventory materials: {ex.Message}");
            }

            // Convert entity list → DTO list
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