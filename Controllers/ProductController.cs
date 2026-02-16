// -------------------------------------------------------------
// PRODUCTS CONTROLLER
// -------------------------------------------------------------
// This controller handles all Product and BOM related HTTP requests.
//
// Flow:
// Client (React/Postman)
// → ProductsController
// → ProductService
// → ProductRepository
// → DbContext
// → Database
// -------------------------------------------------------------

using ManuBackend.DTOs;              // DTO classes
using Microsoft.AspNetCore.Authorization;  // For [Authorize]
using Microsoft.AspNetCore.Mvc;      // ControllerBase, IActionResult
using ManuBackend.Services;          // IProductService



namespace ManuBackend.Controllers
{
    // -------------------------------------------------------------
    // [Authorize]
    // Requires valid JWT token for all endpoints.
    // Currently commented out.
    //
    // [AllowAnonymous]
    // Means anyone can access without token.
    // -------------------------------------------------------------
    //[Authorize]
    [AllowAnonymous]

    // Enables automatic model validation and better API behavior
    [ApiController]

    // Base route:
    // /api/products
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        // Dependency Injection of Service layer
        private readonly IProductService _productService;

        // Constructor
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }



        // =============================================================
        // -------------------- PRODUCT ENDPOINTS -----------------------
        // =============================================================

        // -------------------------------------------------------------
        // GET /api/products?searchTerm=hammer
        //
        // [FromQuery] means value comes from URL query string.
        // Example:
        // /api/products?searchTerm=drill
        // -------------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAllProducts(
            [FromQuery] string? searchTerm = null)
        {
            try
            {
                var products =
                    await _productService.GetAllProductsAsync(searchTerm);

                return Ok(products); // 200 OK
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    new { message = ex.Message });
            }
        }



        // -------------------------------------------------------------
        // GET /api/products/5
        //
        // {id} is route parameter
        // ASP.NET automatically maps it to method parameter
        // -------------------------------------------------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var product =
                    await _productService.GetProductByIdAsync(id);

                // If not found → return 404
                if (product == null)
                    return NotFound(
                        new { message = $"Product with ID {id} not found" });

                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    new { message = ex.Message });
            }
        }



        // -------------------------------------------------------------
        // POST /api/products
        //
        // Creates new product
        // Data comes from request body
        // -------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> CreateProduct(
            [FromBody] CreateProductDto dto)
        {
            try
            {
                var product =
                    await _productService.CreateProductAsync(dto);

                return StatusCode(201, product); // 201 Created
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(
                    new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    new { message = ex.Message });
            }
        }



        // -------------------------------------------------------------
        // PUT /api/products/5
        //
        // Updates product by ID
        // -------------------------------------------------------------
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(
            int id,
            [FromBody] UpdateProductDto dto)
        {
            try
            {
                var product =
                    await _productService.UpdateProductAsync(id, dto);

                return Ok(product);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(
                    new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    new { message = ex.Message });
            }
        }



        // -------------------------------------------------------------
        // DELETE /api/products/5
        //
        // Deletes product by ID
        // -------------------------------------------------------------
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var deleted =
                    await _productService.DeleteProductAsync(id);

                if (!deleted)
                    return NotFound(
                        new { message = $"Product with ID {id} not found" });

                return Ok(
                    new { message = "Product deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    new { message = ex.Message });
            }
        }



        // =============================================================
        // -------------------- BOM ENDPOINTS ---------------------------
        // =============================================================

        // -------------------------------------------------------------
        // GET /api/products/5/boms
        //
        // Nested route:
        // Product → Related BOMs
        // -------------------------------------------------------------
        [HttpGet("{productId}/boms")]
        public async Task<IActionResult>
            GetBOMsByProductId(int productId)
        {
            try
            {
                var boms =
                    await _productService.GetBOMsByProductIdAsync(productId);

                return Ok(boms);
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    new { message = ex.Message });
            }
        }



        // -------------------------------------------------------------
        // POST /api/products/5/boms
        //
        // Creates new BOM under a product
        // -------------------------------------------------------------
        [HttpPost("{productId}/boms")]
        public async Task<IActionResult>
            CreateBOM(int productId,
                      [FromBody] CreateBOMDto dto)
        {
            try
            {
                var bom =
                    await _productService.CreateBOMAsync(productId, dto);

                return StatusCode(201, bom);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(
                    new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    new { message = ex.Message });
            }
        }



        // -------------------------------------------------------------
        // PUT /api/products/boms/10
        //
        // Updates a BOM directly by BOM ID
        // -------------------------------------------------------------
        [HttpPut("boms/{bomId}")]
        public async Task<IActionResult>
            UpdateBOM(int bomId,
                      [FromBody] UpdateBOMDto dto)
        {
            try
            {
                var bom =
                    await _productService.UpdateBOMAsync(bomId, dto);

                return Ok(bom);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(
                    new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    new { message = ex.Message });
            }
        }



        // -------------------------------------------------------------
        // DELETE /api/products/boms/10
        // -------------------------------------------------------------
        [HttpDelete("boms/{bomId}")]
        public async Task<IActionResult> DeleteBOM(int bomId)
        {
            try
            {
                var deleted =
                    await _productService.DeleteBOMAsync(bomId);

                if (!deleted)
                    return NotFound(
                        new { message = $"BOM with ID {bomId} not found" });

                return Ok(
                    new { message = "BOM deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    new { message = ex.Message });
            }
        }



        // -------------------------------------------------------------
        // PUT /api/products/5/boms/replace
        //
        // Replaces entire BOM list for a product
        // Used in EditBOM page
        // -------------------------------------------------------------
        [HttpPut("{productId}/boms/replace")]
        public async Task<IActionResult>
            ReplaceBOMs(int productId,
                        [FromBody] List<CreateBOMDto> boms)
        {
            try
            {
                var replaced =
                    await _productService.ReplaceBOMsAsync(productId, boms);

                return Ok(replaced);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(
                    new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    new { message = ex.Message });
            }
        }
    }
}
