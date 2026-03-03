// =============================================================
// PRODUCTS CONTROLLER
// =============================================================
// This controller is the ENTRY POINT for all Product & BOM APIs
//
// IMPORTANT RULE:
// - Controller NEVER contains business logic
// - Controller NEVER talks to DbContext directly
//
// It ONLY:
// 1️⃣ Accepts HTTP requests
// 2️⃣ Validates input
// 3️⃣ Calls Service layer
// 4️⃣ Returns HTTP responses
//
// REQUEST FLOW:
// Client (React / Postman)
// → ProductsController
// → ProductService (business rules)
// → ProductRepository (database queries)
// → DbContext
// → Database
// =============================================================


// ============================
// USING STATEMENTS
// ============================

// DTOs used for request & response
using ManuBackend.DTOs;

// Service layer interface
using ManuBackend.Services;

// For authorization attributes like [Authorize], [AllowAnonymous]
using Microsoft.AspNetCore.Authorization;

// Base controller features & HTTP responses
using Microsoft.AspNetCore.Mvc;

// Rate limiting support
using Microsoft.AspNetCore.RateLimiting;


namespace ManuBackend.Controllers
{
    // =============================================================
    // AUTHORIZATION CONFIGURATION
    // =============================================================

    // [Authorize]
    // → Requires VALID JWT token for ALL endpoints in this controller
    //
    // [AllowAnonymous]
    // → Anyone can access without token
    //
    // Currently using AllowAnonymous for development/testing
    [Authorize]
   // [AllowAnonymous]

    // -------------------------------------------------------------
    // [ApiController]
    // Enables:
    // ✔ Automatic model validation
    // ✔ Automatic 400 Bad Request on invalid DTO
    // ✔ Automatic JSON → DTO binding
    // ✔ Standard error responses
    // -------------------------------------------------------------
    [ApiController]

    // -------------------------------------------------------------
    // Base route:
    // api/[controller]
    //
    // Controller name = ProductsController
    // [controller] = "products"
    //
    // Base URL:
    // /api/products
    // -------------------------------------------------------------
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        // =============================================================
        // DEPENDENCY INJECTION (SERVICE LAYER)
        // =============================================================

        // private  → accessible only inside this controller
        // readonly → assigned once via constructor
        //
        // IProductService = abstraction
        // Actual implementation injected by ASP.NET Core
        private readonly IProductService _productService;

        // -------------------------------------------------------------
        // CONSTRUCTOR
        // -------------------------------------------------------------
        // ASP.NET Core automatically injects IProductService
        // because it is registered in Program.cs
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }


        // =============================================================
        // =================== PRODUCT ENDPOINTS =======================
        // =============================================================

        // -------------------------------------------------------------
        // GET /api/products?searchTerm=hammer
        // -------------------------------------------------------------
        // Purpose:
        // - Fetch all products
        // - Optionally filter by search term
        //
        // [FromQuery] → value comes from URL query string
        //
        // Example:
        // /api/products?searchTerm=drill
        // -------------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAllProducts(
            [FromQuery] string? searchTerm = null)
        {
            try
            {
                // Call service layer
                var products =
                    await _productService.GetAllProductsAsync(searchTerm);

                // HTTP 200 OK
                return Ok(products);
            }
            catch (Exception ex)
            {
                // HTTP 500 Internal Server Error
                return StatusCode(500,
                    new { message = ex.Message });
            }
        }


        // -------------------------------------------------------------
        // GET /api/products/5
        // -------------------------------------------------------------
        // Purpose:
        // - Fetch single product by ID
        //
        // {id} is a ROUTE PARAMETER
        // ASP.NET automatically maps it to method argument
        // -------------------------------------------------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var product =
                    await _productService.GetProductByIdAsync(id);

                // If product does not exist → 404
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
        // -------------------------------------------------------------
        // Purpose:
        // - Create a new product
        //
        // Data comes from request BODY as JSON
        // -------------------------------------------------------------
        [HttpPost]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> CreateProduct(
            [FromBody] CreateProductDto dto)
        {
            try
            {
                var product =
                    await _productService.CreateProductAsync(dto);

                // HTTP 201 Created
                return StatusCode(201, product);
            }
            catch (InvalidOperationException ex)
            {
                // Business rule failure
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
        // -------------------------------------------------------------
        // Purpose:
        // - Update an existing product
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
                // Product not found
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
        // -------------------------------------------------------------
        // Purpose:
        // - Delete a product by ID
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
        // ====================== BOM ENDPOINTS ========================
        // =============================================================

        // -------------------------------------------------------------
        // GET /api/products/5/boms
        // -------------------------------------------------------------
        // Purpose:
        // - Fetch all BOMs under a specific product
        //
        // This is a NESTED RESOURCE
        // -------------------------------------------------------------
        [HttpGet("{productId}/boms")]
        public async Task<IActionResult> GetBOMsByProductId(int productId)
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
        // -------------------------------------------------------------
        // Purpose:
        // - Create a new BOM under a product
        // -------------------------------------------------------------
        [HttpPost("{productId}/boms")]
        public async Task<IActionResult> CreateBOM(
            int productId,
            [FromBody] CreateBOMDto dt  o)
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
        // -------------------------------------------------------------
        // Purpose:
        // - Update BOM using BOM ID directly
        // -------------------------------------------------------------
        [HttpPut("boms/{bomId}")]
        public async Task<IActionResult> UpdateBOM(
            int bomId,
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
        // -------------------------------------------------------------
        // Purpose:
        // - Replace ENTIRE BOM list for a product
        // - Used in Edit BOM screen
        // -------------------------------------------------------------
        [HttpPut("{productId}/boms/replace")]
        public async Task<IActionResult> ReplaceBOMs(
            int productId,
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