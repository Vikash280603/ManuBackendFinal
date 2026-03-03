using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ManuBackend.Services;
using ManuBackend.DTOs;

namespace ManuBackend.Controllers
{
    // =============================================================
    // WORK ORDER CONTROLLER
    // =============================================================
    // This controller manages the FULL PRODUCTION WORKFLOW
    //
    // Responsibilities:
    // - Create work orders
    // - Track production status
    // - Allocate inventory materials
    // - Mark production complete
    // - Approve quality
    //
    // Flow:
    // Client → Controller → Service → Repository → Database
    // =============================================================

    // [Authorize] can be enabled later when authentication is ready
    [ApiController]

    // Base route:
    // /api/workorder
    [Route("api/[controller]")]
    public class WorkOrderController : ControllerBase
    {
        // Service layer dependency
        private readonly IWorkOrderService _service;

        // Constructor injection
        public WorkOrderController(IWorkOrderService service)
        {
            _service = service;
        }

        // =============================================================
        // READ ENDPOINTS
        // =============================================================

        // -------------------------------------------------------------
        // GET /api/workorder
        // Returns ALL work orders
        // -------------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAllWorkOrders()
        {
            try
            {
                var workOrders = await _service.GetAllWorkOrdersAsync();
                return Ok(workOrders); // 200 OK
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // -------------------------------------------------------------
        // GET /api/workorder/{workOrderId}
        // Fetch a single work order by ID
        // -------------------------------------------------------------
        [HttpGet("{workOrderId}")]
        public async Task<IActionResult> GetWorkOrderById(string workOrderId)
        {
            try
            {
                var workOrder = await _service.GetWorkOrderByIdAsync(workOrderId);

                if (workOrder == null)
                    return NotFound(new
                    {
                        message = $"Work Order {workOrderId} not found"
                    });

                return Ok(workOrder);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // -------------------------------------------------------------
        // GET /api/workorder/status/{status}
        // Filter work orders by status
        // Example: PLANNED, IN_PROGRESS, COMPLETED
        // -------------------------------------------------------------
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetWorkOrdersByStatus(string status)
        {
            try
            {
                var workOrders = await _service.GetWorkOrdersByStatusAsync(status);
                return Ok(workOrders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // =============================================================
        // CREATE ENDPOINTS
        // =============================================================

        // -------------------------------------------------------------
        // POST /api/workorder
        // Creates a SINGLE work order
        // -------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> CreateWorkOrder(
            [FromBody] CreateWorkOrderDto dto)
        {
            try
            {
                var workOrder = await _service.CreateWorkOrderAsync(dto);
                return StatusCode(201, workOrder); // 201 Created
            }
            catch (KeyNotFoundException ex)
            {
                // Product not found
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // -------------------------------------------------------------
        // POST /api/workorder/batch
        // Creates MULTIPLE work orders from one request
        // Used for bulk production planning
        // -------------------------------------------------------------
        [HttpPost("batch")]
        public async Task<IActionResult> CreateBatchWorkOrders(
            [FromBody] CreateBatchDto dto)
        {
            try
            {
                var workOrders = await _service
                    .CreateBatchWorkOrdersAsync(dto.Order, dto.Batches);

                return StatusCode(201, workOrders);
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

        // =============================================================
        // UPDATE ENDPOINTS
        // =============================================================

        // -------------------------------------------------------------
        // PUT /api/workorder/{workOrderId}
        // Updates work order details
        // -------------------------------------------------------------
        [HttpPut("{workOrderId}")]
        public async Task<IActionResult> UpdateWorkOrder(
            string workOrderId,
            [FromBody] UpdateWorkOrderDto dto)
        {
            try
            {
                var workOrder =
                    await _service.UpdateWorkOrderAsync(workOrderId, dto);

                return Ok(workOrder);
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

        // -------------------------------------------------------------
        // DELETE /api/workorder/{workOrderId}
        // Deletes a work order
        // -------------------------------------------------------------
        [HttpDelete("{workOrderId}")]
        public async Task<IActionResult> DeleteWorkOrder(string workOrderId)
        {
            try
            {
                var deleted =
                    await _service.DeleteWorkOrderAsync(workOrderId);

                if (!deleted)
                    return NotFound(new
                    {
                        message = $"Work Order {workOrderId} not found"
                    });

                return Ok(new
                {
                    message = "Work Order deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // =============================================================
        // WORKFLOW / STATE TRANSITION ENDPOINTS
        // =============================================================

        // -------------------------------------------------------------
        // POST /api/workorder/{id}/allocate
        // Allocates inventory materials for production
        // -------------------------------------------------------------
        [HttpPost("{workOrderId}/allocate")]
        public async Task<IActionResult> AllocateMaterials(string workOrderId)
        {
            try
            {
                var workOrder =
                    await _service.AllocateMaterialsAsync(workOrderId);

                return Ok(workOrder);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // Invalid workflow step
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // -------------------------------------------------------------
        // POST /api/workorder/{id}/complete
        // Marks production as COMPLETED
        // -------------------------------------------------------------
        [HttpPost("{workOrderId}/complete")]
        public async Task<IActionResult> CompleteWorkOrder(string workOrderId)
        {
            try
            {
                var workOrder =
                    await _service.CompleteWorkOrderAsync(workOrderId);

                return Ok(workOrder);
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

        // -------------------------------------------------------------
        // POST /api/workorder/{id}/approve-quality
        // Final approval after Quality Check PASS
        // -------------------------------------------------------------
        [HttpPost("{workOrderId}/approve-quality")]
        public async Task<IActionResult> ApproveQuality(string workOrderId)
        {
            try
            {
                var workOrder =
                    await _service.ApproveQualityAsync(workOrderId);

                return Ok(workOrder);
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
    }

    // =============================================================
    // HELPER DTO
    // =============================================================

    // Used for batch work order creation
    public class CreateBatchDto
    {
        // Base order details
        public CreateWorkOrderDto Order { get; set; } = new();

        // Number of batches to create
        public int Batches { get; set; }
    }
}