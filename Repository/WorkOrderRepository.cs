using ManuBackend.Data;
using ManuBackend.Models;
using ManuBackend.Repository;
using Microsoft.EntityFrameworkCore;


namespace ManuBackend.Repository
{
    public class WorkOrderRepository : IWorkOrderRepository
    {
        private readonly AppDbContext _context;

        public WorkOrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<WorkOrder>> GetAllWorkOrdersAsync()
        {
            return await _context.WorkOrders
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();
        }

        public async Task<WorkOrder?> GetWorkOrderByIdAsync(string workOrderId)
        {
            return await _context.WorkOrders
                .FirstOrDefaultAsync(w => w.WorkOrderId == workOrderId);
        }

        public async Task<List<WorkOrder>> GetWorkOrdersByStatusAsync(string status)
        {
            return await _context.WorkOrders
                .Where(w => w.Status == status)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();
        }

        public async Task<WorkOrder> CreateWorkOrderAsync(WorkOrder workOrder)
        {
            _context.WorkOrders.Add(workOrder);
            await _context.SaveChangesAsync();
            return workOrder;
        }

        public async Task<WorkOrder> UpdateWorkOrderAsync(WorkOrder workOrder)
        {
            _context.WorkOrders.Update(workOrder);
            await _context.SaveChangesAsync();
            return workOrder;
        }

        public async Task<bool> DeleteWorkOrderAsync(string workOrderId)
        {
            var workOrder = await _context.WorkOrders.FindAsync(workOrderId);
            if (workOrder == null) return false;

            _context.WorkOrders.Remove(workOrder);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}