using ManuBackend.Models;


namespace ManuBackend.Repository
{
    public interface IWorkOrderRepository
    {
        Task<List<WorkOrder>> GetAllWorkOrdersAsync();
        Task<WorkOrder?> GetWorkOrderByIdAsync(string workOrderId);
        Task<List<WorkOrder>> GetWorkOrdersByStatusAsync(string status);
        Task<WorkOrder> CreateWorkOrderAsync(WorkOrder workOrder);
        Task<WorkOrder> UpdateWorkOrderAsync(WorkOrder workOrder);
        Task<bool> DeleteWorkOrderAsync(string workOrderId);
    }
}