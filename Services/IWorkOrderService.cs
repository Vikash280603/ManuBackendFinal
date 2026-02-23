using ManuBackend.DTOs;

namespace ManuBackend.Services
{
    public interface IWorkOrderService
    {
        Task<List<WorkOrderDto>> GetAllWorkOrdersAsync();
        Task<WorkOrderDto?> GetWorkOrderByIdAsync(string workOrderId);
        Task<List<WorkOrderDto>> GetWorkOrdersByStatusAsync(string status);
        Task<WorkOrderDto> CreateWorkOrderAsync(CreateWorkOrderDto dto);
        Task<List<WorkOrderDto>> CreateBatchWorkOrdersAsync(CreateWorkOrderDto dto, int batches);
        Task<WorkOrderDto> UpdateWorkOrderAsync(string workOrderId, UpdateWorkOrderDto dto);
        Task<bool> DeleteWorkOrderAsync(string workOrderId);

        // Workflow operations  
        Task<WorkOrderDto> AllocateMaterialsAsync(string workOrderId);
        Task<WorkOrderDto> CompleteWorkOrderAsync(string workOrderId);
        Task<WorkOrderDto> ApproveQualityAsync(string workOrderId);
    }
}
