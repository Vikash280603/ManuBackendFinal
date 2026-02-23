using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ManuBackend.Controllers;


using ManuBackend.Services;
using ManuBackend.DTOs;

namespace ManuBackend.Controllers
{
    //[Authorize] lets have it later !
    [ApiController]
    [Route("api/[controller]")]
    public class WorkOrderController: ControllerBase

    {
        private readonly IWorkOrderService _service;

        public WorkOrderController(IWorkOrderService service)
        {
            _service = service;
        }

        // -------------------- READ ENDPOINTS --------------------  

        // GET /api/workorder  
        [HttpGet]
        public async Task<IActionResult> GetAllWorkOrders()
        {
            try
            {
                var workOrders = await _service.GetAllWorkOrdersAsync();
                return Ok(workOrders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET /api/workorder/{id}  
        [HttpGet("{workOrderId}")]
        public async Task<IActionResult> GetWorkOrderById(string workOrderId)
        {
            try
            {
                var workOrder = await _service.GetWorkOrderByIdAsync(workOrderId);
                if (workOrder == null)
                    return NotFound(new { message = $"Work Order {workOrderId} not found" });

                return Ok(workOrder);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // GET /api/workorder/status/{status}  
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

        // -------------------- CREATE ENDPOINTS --------------------  

        // POST /api/workorder  
        [HttpPost]
        public async Task<IActionResult> CreateWorkOrder([FromBody] CreateWorkOrderDto dto)
        {
            try
            {
                var workOrder = await _service.CreateWorkOrderAsync(dto);
                return StatusCode(201, workOrder);
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

        // POST /api/workorder/batch  
        [HttpPost("batch")]
        public async Task<IActionResult> CreateBatchWorkOrders([FromBody] CreateBatchDto dto)
        {
            try
            {
                var workOrders = await _service.CreateBatchWorkOrdersAsync(dto.Order, dto.Batches);
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

        // -------------------- UPDATE ENDPOINTS --------------------  

        // PUT /api/workorder/{id}  
        [HttpPut("{workOrderId}")]
        public async Task<IActionResult> UpdateWorkOrder(string workOrderId, [FromBody] UpdateWorkOrderDto dto)
        {
            try
            {
                var workOrder = await _service.UpdateWorkOrderAsync(workOrderId, dto);
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

        // DELETE /api/workorder/{id}  
        [HttpDelete("{workOrderId}")]
        public async Task<IActionResult> DeleteWorkOrder(string workOrderId)
        {
            try
            {
                var deleted = await _service.DeleteWorkOrderAsync(workOrderId);
                if (!deleted)
                    return NotFound(new { message = $"Work Order {workOrderId} not found" });

                return Ok(new { message = "Work Order deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // -------------------- WORKFLOW ENDPOINTS --------------------  

        // POST /api/workorder/{id}/allocate  
        [HttpPost("{workOrderId}/allocate")]
        public async Task<IActionResult> AllocateMaterials(string workOrderId)
        {
            try
            {
                var workOrder = await _service.AllocateMaterialsAsync(workOrderId);
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

        // POST /api/workorder/{id}/complete  
        [HttpPost("{workOrderId}/complete")]
        public async Task<IActionResult> CompleteWorkOrder(string workOrderId)
        {
            try
            {
                var workOrder = await _service.CompleteWorkOrderAsync(workOrderId);
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

        // POST /api/workorder/{id}/approve-quality  
        [HttpPost("{workOrderId}/approve-quality")]
        public async Task<IActionResult> ApproveQuality(string workOrderId)
        {
            try
            {
                var workOrder = await _service.ApproveQualityAsync(workOrderId);
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

    // Helper DTO for batch creation  
    public class CreateBatchDto
    {
        public CreateWorkOrderDto Order { get; set; } = new();
        public int Batches { get; set; }
    }
}