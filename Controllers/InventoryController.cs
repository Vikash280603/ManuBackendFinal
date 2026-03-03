using ManuBackend.DTOs;
using ManuBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManuBackend.Controllers
{
    // [Authorize] → User MUST be authenticated to access this controller
    [Authorize]

    // [ApiController] → Enables:
    // - Automatic model validation
    // - Automatic 400 response for invalid models
    // - Better API behavior
    [ApiController]

    // Base route:
    // api/inventory
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        // Service layer dependency
        // Controller NEVER talks to database directly
        private readonly IInventoryService _service;

        // Constructor Injection (Dependency Injection)
        public InventoryController(IInventoryService service)
        {
            _service = service;
        }

        // -------------------- READ ENDPOINTS --------------------

        // GET /api/inventory
        [HttpGet]
        public async Task<IActionResult> GetAllInventories()
        {
            try
            {
                var inventories = await _service.GetAllInventoriesAsync();
                return Ok(inventories); // 200 OK
            }
            catch (Exception ex)
            {
                // 500 Internal Server Error
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET /api/inventory/5
        [HttpGet("{inventoryId}")]
        public async Task<IActionResult> GetInventoryById(int inventoryId)
        {
            try
            {
                var inventory = await _service.GetInventoryByIdAsync(inventoryId);

                if (inventory == null)
                    return NotFound(new { message = $"Inventory {inventoryId} not found" });

                return Ok(inventory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET /api/inventory/product/5
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetInventoriesByProductId(int productId)
        {
            try
            {
                var inventories = await _service.GetInventoriesByProductIdAsync(productId);
                return Ok(inventories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // -------------------- UPDATE ENDPOINTS --------------------

        // PUT /api/inventory/materials/10
        [HttpPut("materials/{materialId}")]
        public async Task<IActionResult> UpdateMaterial(
            int materialId,
            [FromBody] UpdateInventoryMaterialDto dto)
        {
            try
            {
                var material = await _service.UpdateMaterialAsync(materialId, dto);
                return Ok(material);
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

        // POST /api/inventory/materials/10/adjust
        [HttpPost("materials/{materialId}/adjust")]
        public async Task<IActionResult> AdjustMaterialQuantity(
            int materialId,
            [FromBody] AdjustQuantityDto dto)
        {
            try
            {
                // Delta can be +ve or -ve
                var material = await _service.AdjustMaterialQuantityAsync(
                    materialId,
                    dto.Delta);

                return Ok(material);
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

        // GET /api/inventory/lowstock
        [HttpGet("lowstock")]
        public async Task<IActionResult> GetLowStockMaterials()
        {
            try
            {
                var materials = await _service.GetLowStockMaterialsAsync();
                return Ok(materials);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // -------------------- ADMIN ENDPOINTS --------------------

        // POST /api/inventory/generate
        // Only ADMIN can access
        [HttpPost("generate")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> GenerateInventories()
        {
            try
            {
                var count = await _service.GenerateInventoriesAsync();
                return Ok(new
                {
                    message = $"Generated {count} inventory records",
                    count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // POST /api/inventory/sync/5
        // ADMIN or PRODUCT_BOM_MANAGER can access
        [HttpPost("sync/{productId}")]
        [Authorize(Roles = "admin,product_bom_manager")]
        public async Task<IActionResult> SyncInventoryMaterials(int productId)
        {
            try
            {
                await _service.SyncInventoryMaterialsAsync(productId);
                return Ok(new { message = $"Synced inventory for product {productId}" });
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

    // Simple DTO used only for adjusting quantity
    public class AdjustQuantityDto
    {
        // +10 → add stock
        // -5  → reduce stock
        public int Delta { get; set; }
    }
}