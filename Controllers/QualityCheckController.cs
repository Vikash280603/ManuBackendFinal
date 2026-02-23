
using ManuBackend.DTOs;
using ManuBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace ManuBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class QualityCheckController : ControllerBase
    {
        private readonly IQualityCheckService _service;

        public QualityCheckController(IQualityCheckService service)
        {
            _service = service;
        }

        // -------------------- READ ENDPOINTS --------------------

        // GET /api/qualitycheck
        [HttpGet]
        public async Task<IActionResult> GetAllQualityChecks()
        {
            try
            {
                var qualityChecks = await _service.GetAllQualityChecksAsync();
                return Ok(qualityChecks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET /api/qualitycheck/{id}
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

        // GET /api/qualitycheck/result/{result}
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

        // -------------------- CREATE ENDPOINT --------------------

        // POST /api/qualitycheck
        [HttpPost]
        public async Task<IActionResult> CreateQualityCheck([FromBody] CreateQualityCheckDto dto)
        {
            try
            {
                var qualityCheck = await _service.CreateQualityCheckAsync(dto);
                return StatusCode(201, qualityCheck);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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

        // -------------------- DELETE ENDPOINT --------------------

        // DELETE /api/qualitycheck/{id}
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