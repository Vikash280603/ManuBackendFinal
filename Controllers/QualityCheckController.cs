// =============================================================
// QUALITY CHECK CONTROLLER
// =============================================================
//
// A Controller is the ENTRY POINT for HTTP requests
//
// Responsibilities:
// - Accept HTTP requests (GET, POST, DELETE)
// - Validate request data
// - Call Service layer
// - Return HTTP responses (200, 404, 500, etc.)
//
// IMPORTANT INTERVIEW POINTS:
// ❌ No database logic here
// ❌ No business logic here
// ✅ Only request handling & response formatting
// =============================================================

using ManuBackend.DTOs;
using ManuBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManuBackend.Controllers
{
    // -------------------- SECURITY --------------------
    // [Authorize] → User MUST be authenticated (JWT token required)
    [Authorize]

    // [ApiController] →
    // - Enables automatic model validation
    // - Automatically binds request body to DTOs
    // - Returns 400 BadRequest if model validation fails
    [ApiController]

    // Base route:
    // api/qualitycheck
    [Route("api/[controller]")]
    public class QualityCheckController : ControllerBase
    {
        // Service layer dependency
        // Controller talks ONLY to service, never repository
        private readonly IQualityCheckService _service;

        // -------------------- CONSTRUCTOR --------------------
        // Dependency Injection injects the service implementation
        public QualityCheckController(IQualityCheckService service)
        {
            _service = service;
        }

        // =====================================================
        // READ ENDPOINTS (GET)
        // =====================================================

        // -------------------- GET ALL QUALITY CHECKS --------------------
        // GET /api/qualitycheck
        [HttpGet]
        public async Task<IActionResult> GetAllQualityChecks()
        {
            try
            {
                var qualityChecks = await _service.GetAllQualityChecksAsync();
                return Ok(qualityChecks); // 200 OK
            }
            catch (Exception ex)
            {
                // 500 Internal Server Error
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // -------------------- GET QUALITY CHECK BY ID --------------------
        // GET /api/qualitycheck/{qcId}
        [HttpGet("{qcId}")]
        public async Task<IActionResult> GetQualityCheckById(string qcId)
        {
            try
            {
                var qualityCheck = await _service.GetQualityCheckByIdAsync(qcId);

                if (qualityCheck == null)
                    return NotFound(new { message = $"Quality Check {qcId} not found" });

                return Ok(qualityCheck);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // -------------------- GET QUALITY CHECK BY WORK ORDER --------------------
        // GET /api/qualitycheck/workorder/{workOrderId}
        [HttpGet("workorder/{workOrderId}")]
        public async Task<IActionResult> GetQualityCheckByWorkOrderId(string workOrderId)
        {
            try
            {
                var qualityCheck = await _service.GetQualityCheckByWorkOrderIdAsync(workOrderId);

                if (qualityCheck == null)
                    return NotFound(new { message = $"No quality check found for work order {workOrderId}" });

                return Ok(qualityCheck);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // -------------------- GET QUALITY CHECKS BY RESULT --------------------
        // GET /api/qualitycheck/result/{result}
        // Example results: PASS / FAIL / REWORK
        [HttpGet("result/{result}")]
        public async Task<IActionResult> GetQualityChecksByResult(string result)
        {
            try
            {
                var qualityChecks = await _service.GetQualityChecksByResultAsync(result);
                return Ok(qualityChecks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // =====================================================
        // CREATE ENDPOINT (POST)
        // =====================================================

        // -------------------- CREATE QUALITY CHECK --------------------
        // POST /api/qualitycheck
        // [FromBody] → Data comes from request body (JSON)
        [HttpPost]
        public async Task<IActionResult> CreateQualityCheck([FromBody] CreateQualityCheckDto dto)
        {
            try
            {
                var qualityCheck = await _service.CreateQualityCheckAsync(dto);

                // 201 Created
                return StatusCode(201, qualityCheck);
            }
            catch (KeyNotFoundException ex)
            {
                // Related entity (like WorkOrder) not found
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // Business rule violation
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // =====================================================
        // DELETE ENDPOINT
        // =====================================================

        // -------------------- DELETE QUALITY CHECK --------------------
        // DELETE /api/qualitycheck/{qcId}
        [HttpDelete("{qcId}")]
        public async Task<IActionResult> DeleteQualityCheck(string qcId)
        {
            try
            {
                var deleted = await _service.DeleteQualityCheckAsync(qcId);

                if (!deleted)
                    return NotFound(new { message = $"Quality Check {qcId} not found" });

                return Ok(new { message = "Quality check deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}