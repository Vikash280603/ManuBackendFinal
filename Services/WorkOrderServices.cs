using ManuBackend.DTOs;
using ManuBackend.Models;
using ManuBackend.Repository;
using ManuBackend.Services;

namespace ManuBackend.Services
{
    // SERVICE LAYER
    // --------------------
    // This class contains BUSINESS LOGIC
    // It coordinates between repositories and applies rules/workflows
    public class WorkOrderService : IWorkOrderService
    {
        // Repositories injected via Dependency Injection
        private readonly IWorkOrderRepository _workOrderRepo;
        private readonly IProductRepository _productRepo;
        private readonly IInventoryRepository _inventoryRepo;

        // Constructor Injection
        public WorkOrderService(
            IWorkOrderRepository workOrderRepo,
            IProductRepository productRepo,
            IInventoryRepository inventoryRepo)
        {
            _workOrderRepo = workOrderRepo;
            _productRepo = productRepo;
            _inventoryRepo = inventoryRepo;
        }

        // -------------------- READ OPERATIONS --------------------

        // Fetch ALL work orders and convert Entity → DTO
        public async Task<List<WorkOrderDto>> GetAllWorkOrdersAsync()
        {
            var workOrders = await _workOrderRepo.GetAllWorkOrdersAsync();

            // Mapping Entity → DTO
            return workOrders.Select(w => new WorkOrderDto
            {
                WorkOrderId = w.WorkOrderId,
                ProductId = w.ProductId,
                Quantity = w.Quantity,
                Status = w.Status,
                ScheduledDate = w.ScheduledDate,
                CreatedAt = w.CreatedAt,
                CompletedAt = w.CompletedAt
            }).ToList();
        }

        // Fetch ONE work order by ID
        public async Task<WorkOrderDto?> GetWorkOrderByIdAsync(string workOrderId)
        {
            var workOrder = await _workOrderRepo.GetWorkOrderByIdAsync(workOrderId);
            if (workOrder == null) return null;

            return new WorkOrderDto
            {
                WorkOrderId = workOrder.WorkOrderId,
                ProductId = workOrder.ProductId,
                Quantity = workOrder.Quantity,
                Status = workOrder.Status,
                ScheduledDate = workOrder.ScheduledDate,
                CreatedAt = workOrder.CreatedAt,
                CompletedAt = workOrder.CompletedAt
            };
        }

        // Fetch work orders filtered by status
        public async Task<List<WorkOrderDto>> GetWorkOrdersByStatusAsync(string status)
        {
            var workOrders = await _workOrderRepo.GetWorkOrdersByStatusAsync(status);

            return workOrders.Select(w => new WorkOrderDto
            {
                WorkOrderId = w.WorkOrderId,
                ProductId = w.ProductId,
                Quantity = w.Quantity,
                Status = w.Status,
                ScheduledDate = w.ScheduledDate,
                CreatedAt = w.CreatedAt,
                CompletedAt = w.CompletedAt
            }).ToList();
        }

        // -------------------- CREATE OPERATIONS --------------------

        // Create a single work order
        public async Task<WorkOrderDto> CreateWorkOrderAsync(CreateWorkOrderDto dto)
        {
            // BUSINESS RULE:
            // A work order cannot exist without a valid product
            var product = await _productRepo.GetProductByIdAsync(dto.ProductId);
            if (product == null)
                throw new KeyNotFoundException($"Product {dto.ProductId} not found");

            // Create WorkOrder entity
            var workOrder = new WorkOrder
            {
                WorkOrderId = Guid.NewGuid().ToString(), // Unique ID
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                Status = "PLANNED", // Initial workflow state
                ScheduledDate = dto.ScheduledDate,
                CreatedAt = DateTime.UtcNow,
                CompletedAt = null
            };

            var saved = await _workOrderRepo.CreateWorkOrderAsync(workOrder);

            return new WorkOrderDto
            {
                WorkOrderId = saved.WorkOrderId,
                ProductId = saved.ProductId,
                Quantity = saved.Quantity,
                Status = saved.Status,
                ScheduledDate = saved.ScheduledDate,
                CreatedAt = saved.CreatedAt,
                CompletedAt = saved.CompletedAt
            };
        }

        // Create multiple work orders in batch
        public async Task<List<WorkOrderDto>> CreateBatchWorkOrdersAsync(
            CreateWorkOrderDto dto, int batches)
        {
            var createdOrders = new List<WorkOrderDto>();

            // Loop-based batch creation
            for (int i = 0; i < batches; i++)
            {
                var order = await CreateWorkOrderAsync(dto);
                createdOrders.Add(order);
            }

            return createdOrders;
        }

        // -------------------- UPDATE OPERATIONS --------------------

        public async Task<WorkOrderDto> UpdateWorkOrderAsync(
            string workOrderId, UpdateWorkOrderDto dto)
        {
            var workOrder = await _workOrderRepo.GetWorkOrderByIdAsync(workOrderId);
            if (workOrder == null)
                throw new KeyNotFoundException($"Work Order {workOrderId} not found");

            // Partial update pattern
            if (!string.IsNullOrWhiteSpace(dto.Status))
                workOrder.Status = dto.Status;

            if (dto.Quantity.HasValue)
                workOrder.Quantity = dto.Quantity.Value;

            if (dto.ScheduledDate.HasValue)
                workOrder.ScheduledDate = dto.ScheduledDate;

            var updated = await _workOrderRepo.UpdateWorkOrderAsync(workOrder);

            return new WorkOrderDto
            {
                WorkOrderId = updated.WorkOrderId,
                ProductId = updated.ProductId,
                Quantity = updated.Quantity,
                Status = updated.Status,
                ScheduledDate = updated.ScheduledDate,
                CreatedAt = updated.CreatedAt,
                CompletedAt = updated.CompletedAt
            };
        }

        public async Task<bool> DeleteWorkOrderAsync(string workOrderId)
        {
            return await _workOrderRepo.DeleteWorkOrderAsync(workOrderId);
        }

        // -------------------- WORKFLOW OPERATIONS --------------------

        // PLANNED → IN_PROGRESS
        public async Task<WorkOrderDto> AllocateMaterialsAsync(string workOrderId)
        {
            var workOrder = await _workOrderRepo.GetWorkOrderByIdAsync(workOrderId);
            if (workOrder == null)
                throw new KeyNotFoundException($"Work Order {workOrderId} not found");

            // Workflow validation
            if (workOrder.Status != "PLANNED")
                throw new InvalidOperationException(
                    "Only PLANNED orders can allocate materials");

            // Fetch product + BOM
            var product = await _productRepo.GetProductByIdAsync(workOrder.ProductId);
            if (product == null)
                throw new KeyNotFoundException(
                    $"Product {workOrder.ProductId} not found");

            // Fetch inventory for product
            var inventories = await _inventoryRepo
                .GetInventoriesByProductIdAsync(workOrder.ProductId);

            if (!inventories.Any())
                throw new InvalidOperationException(
                    "No inventory found for this product");

            var inventory = inventories.First(); // Assume first location

            // VALIDATION: Check material availability
            foreach (var bom in product.BOMs)
            {
                var material = inventory.Materials
                    .FirstOrDefault(m => m.MaterialName == bom.MaterialName);

                var required = bom.Quantity * workOrder.Quantity;

                if (material == null || material.AvailableQty < required)
                {
                    throw new InvalidOperationException(
                        $"Insufficient inventory for {bom.MaterialName}. " +
                        $"Required: {required}, Available: {material?.AvailableQty ?? 0}"
                    );
                }
            }

            // DEDUCTION: Reduce inventory quantities
            foreach (var bom in product.BOMs)
            {
                var material = inventory.Materials
                    .First(m => m.MaterialName == bom.MaterialName);

                var required = bom.Quantity * workOrder.Quantity;
                material.AvailableQty -= required;

                await _inventoryRepo.UpdateMaterialAsync(material);
            }

            // Update work order status
            workOrder.Status = "IN_PROGRESS";
            var updated = await _workOrderRepo.UpdateWorkOrderAsync(workOrder);

            return new WorkOrderDto
            {
                WorkOrderId = updated.WorkOrderId,
                ProductId = updated.ProductId,
                Quantity = updated.Quantity,
                Status = updated.Status,
                ScheduledDate = updated.ScheduledDate,
                CreatedAt = updated.CreatedAt,
                CompletedAt = updated.CompletedAt
            };
        }

        // IN_PROGRESS → COMPLETED
        public async Task<WorkOrderDto> CompleteWorkOrderAsync(string workOrderId)
        {
            var workOrder = await _workOrderRepo.GetWorkOrderByIdAsync(workOrderId);
            if (workOrder == null)
                throw new KeyNotFoundException($"Work Order {workOrderId} not found");

            if (workOrder.Status != "IN_PROGRESS")
                throw new InvalidOperationException(
                    "Only IN_PROGRESS orders can be completed");

            workOrder.Status = "COMPLETED";
            workOrder.CompletedAt = DateTime.UtcNow;

            var updated = await _workOrderRepo.UpdateWorkOrderAsync(workOrder);

            return new WorkOrderDto
            {
                WorkOrderId = updated.WorkOrderId,
                ProductId = updated.ProductId,
                Quantity = updated.Quantity,
                Status = updated.Status,
                ScheduledDate = updated.ScheduledDate,
                CreatedAt = updated.CreatedAt,
                CompletedAt = updated.CompletedAt
            };
        }

        // COMPLETED → QUALITY_DONE
        public async Task<WorkOrderDto> ApproveQualityAsync(string workOrderId)
        {
            var workOrder = await _workOrderRepo.GetWorkOrderByIdAsync(workOrderId);
            if (workOrder == null)
                throw new KeyNotFoundException($"Work Order {workOrderId} not found");

            if (workOrder.Status != "COMPLETED")
                throw new InvalidOperationException(
                    "Only COMPLETED orders can be quality approved");

            workOrder.Status = "QUALITY_DONE";

            var updated = await _workOrderRepo.UpdateWorkOrderAsync(workOrder);

            return new WorkOrderDto
            {
                WorkOrderId = updated.WorkOrderId,
                ProductId = updated.ProductId,
                Quantity = updated.Quantity,
                Status = updated.Status,
                ScheduledDate = updated.ScheduledDate,
                CreatedAt = updated.CreatedAt,
                CompletedAt = updated.CompletedAt
            };
        }
    }
}