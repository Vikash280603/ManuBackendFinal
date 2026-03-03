using ManuBackend.Data;
using ManuBackend.Models;
using ManuBackend.Repository;
using Microsoft.EntityFrameworkCore;

namespace ManuBackend.Repository
{
    // Repository = Data Access Layer
    // This class ONLY talks to the database
    // NO business logic should be written here (important for interviews)
    public class WorkOrderRepository : IWorkOrderRepository
    {
        // DbContext represents the database session
        private readonly AppDbContext _context;

        // Constructor Injection
        // ASP.NET Core automatically injects AppDbContext here
        public WorkOrderRepository(AppDbContext context)
        {
            _context = context;
        }

        // -------------------- READ OPERATIONS --------------------

        // Fetches ALL work orders from database
        // Sorted by latest created first
        public async Task<List<WorkOrder>> GetAllWorkOrdersAsync()
        {
            return await _context.WorkOrders
                .OrderByDescending(w => w.CreatedAt) // Latest first
                .ToListAsync(); // Executes SQL query and returns List<WorkOrder>
        }

        // Fetch ONE work order using WorkOrderId
        // Returns null if not found
        public async Task<WorkOrder?> GetWorkOrderByIdAsync(string workOrderId)
        {
            return await _context.WorkOrders
                .FirstOrDefaultAsync(w => w.WorkOrderId == workOrderId);
        }

        // Fetch work orders filtered by Status (e.g. CREATED, ALLOCATED, COMPLETED)
        public async Task<List<WorkOrder>> GetWorkOrdersByStatusAsync(string status)
        {
            return await _context.WorkOrders
                .Where(w => w.Status == status) // SQL WHERE clause
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();
        }

        // -------------------- CREATE --------------------

        // Inserts a new WorkOrder into database
        public async Task<WorkOrder> CreateWorkOrderAsync(WorkOrder workOrder)
        {
            _context.WorkOrders.Add(workOrder); // Marks entity as Added
            await _context.SaveChangesAsync();  // Executes INSERT query
            return workOrder;
        }

        // -------------------- UPDATE --------------------

        // Updates an existing WorkOrder
        public async Task<WorkOrder> UpdateWorkOrderAsync(WorkOrder workOrder)
        {
            _context.WorkOrders.Update(workOrder); // Marks entity as Modified
            await _context.SaveChangesAsync();     // Executes UPDATE query
            return workOrder;
        }

        // -------------------- DELETE --------------------

        // Deletes a WorkOrder by ID
        // Returns false if record not found
        public async Task<bool> DeleteWorkOrderAsync(string workOrderId)
        {
            // FindAsync uses Primary Key lookup (fast)
            var workOrder = await _context.WorkOrders.FindAsync(workOrderId);

            if (workOrder == null)
                return false;

            _context.WorkOrders.Remove(workOrder); // Marks entity as Deleted
            await _context.SaveChangesAsync();     // Executes DELETE query
            return true;
        }
    }
}