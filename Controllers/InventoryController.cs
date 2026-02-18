// API Controller for Inventory operations

using ManuBackend.DTOs;
using ManuBackend.Services;
using ManuBackend.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace YourProject.Api.Modules.Inventory.Controllers
{
    [Authorize] // Require JWT authentication
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _service;

        public InventoryController(IInventoryService service)
        {
            _service = service;
        }

        // -------------------- INVENTORY ENDPOINTS --------------------

        // GET /api/inventory
        [HttpGet]
        public async Task<IActionResult> GetAllInventories()
        {
            try
            {
                var inventories = await _service.GetAllInventoriesAsync();
                return Ok(inventories);
            }
            catch (Exception ex)
            {
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

        // POST /api/inventory
        [HttpPost]
        public async Task<IActionResult> CreateInventory([FromBody] CreateInventoryDto dto)
        {
            try
            {
                var inventory = await _service.CreateInventoryAsync(dto);
                return StatusCode(201, inventory);
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

        // PUT /api/inventory/5
        [HttpPut("{inventoryId}")]
        public async Task<IActionResult> UpdateInventory(int inventoryId, [FromBody] UpdateInventoryDto dto)
        {
            try
            {
                var inventory = await _service.UpdateInventoryAsync(inventoryId, dto);
                return Ok(inventory);
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

        // DELETE /api/inventory/5
        [HttpDelete("{inventoryId}")]
        public async Task<IActionResult> DeleteInventory(int inventoryId)
        {
            try
            {
                var deleted = await _service.DeleteInventoryAsync(inventoryId);
                if (!deleted)
                    return NotFound(new { message = $"Inventory {inventoryId} not found" });

                return Ok(new { message = "Inventory deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // -------------------- MATERIAL ENDPOINTS --------------------

        // GET /api/inventory/5/materials
        [HttpGet("{inventoryId}/materials")]
        public async Task<IActionResult> GetMaterialsByInventoryId(int inventoryId)
        {
            try
            {
                var materials = await _service.GetMaterialsByInventoryIdAsync(inventoryId);
                return Ok(materials);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // POST /api/inventory/5/materials
        [HttpPost("{inventoryId}/materials")]
        public async Task<IActionResult> CreateMaterial(int inventoryId, [FromBody] CreateInventoryMaterialDto dto)
        {
            try
            {
                var material = await _service.CreateMaterialAsync(inventoryId, dto);
                return StatusCode(201, material);
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

        // PUT /api/inventory/materials/10
        [HttpPut("materials/{materialId}")]
        public async Task<IActionResult> UpdateMaterial(int materialId, [FromBody] UpdateInventoryMaterialDto dto)
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

        // DELETE /api/inventory/materials/10
        [HttpDelete("materials/{materialId}")]
        public async Task<IActionResult> DeleteMaterial(int materialId)
        {
            try
            {
                var deleted = await _service.DeleteMaterialAsync(materialId);
                if (!deleted)
                    return NotFound(new { message = $"Material {materialId} not found" });

                return Ok(new { message = "Material deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // -------------------- SPECIAL ENDPOINTS --------------------

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

        // POST /api/inventory/materials/10/adjust
        // Body: { "delta": 1 } or { "delta": -1 }
        [HttpPost("materials/{materialId}/adjust")]
        public async Task<IActionResult> AdjustMaterialQuantity(int materialId, [FromBody] AdjustQuantityDto dto)
        {
            try
            {
                var material = await _service.AdjustMaterialQuantityAsync(materialId, dto.Delta);
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
    }

    // Helper DTO for adjust endpoint
    public class AdjustQuantityDto
    {
        public int Delta { get; set; } // +1 or -1
    }
}
