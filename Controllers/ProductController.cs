// API Controller for Products and BOMs  
// Defines all HTTP endpoints  

using ManuBackend.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ManuBackend.Services; 

namespace ManuBackend.Controllers
{
    //[Authorize] // Require JWT authentication for all endpoints
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // -------------------- PRODUCT ENDPOINTS --------------------  

        // GET /api/products?searchTerm=hammer  
        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] string? searchTerm = null)
        {
            try
            {
                var products = await _productService.GetAllProductsAsync(searchTerm);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET /api/products/5  
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                    return NotFound(new { message = $"Product with ID {id} not found" });

                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // POST /api/products  
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
        {
            try
            {
                var product = await _productService.CreateProductAsync(dto);
                return StatusCode(201, product); // 201 Created  
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // PUT /api/products/5  
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto dto)
        {
            try
            {
                var product = await _productService.UpdateProductAsync(id, dto);
                return Ok(product);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // DELETE /api/products/5  
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var deleted = await _productService.DeleteProductAsync(id);
                if (!deleted)
                    return NotFound(new { message = $"Product with ID {id} not found" });

                return Ok(new { message = "Product deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // -------------------- BOM ENDPOINTS --------------------  

        // GET /api/products/5/boms  
        [HttpGet("{productId}/boms")]
        public async Task<IActionResult> GetBOMsByProductId(int productId)
        {
            try
            {
                var boms = await _productService.GetBOMsByProductIdAsync(productId);
                return Ok(boms);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // POST /api/products/5/boms  
        [HttpPost("{productId}/boms")]
        public async Task<IActionResult> CreateBOM(int productId, [FromBody] CreateBOMDto dto)
        {
            try
            {
                var bom = await _productService.CreateBOMAsync(productId, dto);
                return StatusCode(201, bom);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // PUT /api/products/boms/10  
        [HttpPut("boms/{bomId}")]
        public async Task<IActionResult> UpdateBOM(int bomId, [FromBody] UpdateBOMDto dto)
        {
            try
            {
                var bom = await _productService.UpdateBOMAsync(bomId, dto);
                return Ok(bom);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // DELETE /api/products/boms/10  
        [HttpDelete("boms/{bomId}")]
        public async Task<IActionResult> DeleteBOM(int bomId)
        {
            try
            {
                var deleted = await _productService.DeleteBOMAsync(bomId);
                if (!deleted)
                    return NotFound(new { message = $"BOM with ID {bomId} not found" });

                return Ok(new { message = "BOM deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // PUT /api/products/5/boms/replace  
        // Used in EditBOM page - replaces entire BOM list  
        [HttpPut("{productId}/boms/replace")]
        public async Task<IActionResult> ReplaceBOMs(int productId, [FromBody] List<CreateBOMDto> boms)
        {
            try
            {
                var replaced = await _productService.ReplaceBOMsAsync(productId, boms);
                return Ok(replaced);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}