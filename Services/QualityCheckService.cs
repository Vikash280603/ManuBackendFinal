using ManuBackend.DTOs;
using ManuBackend.Models;
using ManuBackend.Repository;
using ManuBackend.Services;


namespace ManuBackend.Services
{
    public class QualityCheckService : IQualityCheckService
    {
        private readonly IQualityCheckRepository _qcRepo;
        private readonly IWorkOrderRepository _workOrderRepo;

        public QualityCheckService(
            IQualityCheckRepository qcRepo,
            IWorkOrderRepository workOrderRepo)
        {
            _qcRepo = qcRepo;
            _workOrderRepo = workOrderRepo;
        }

        // -------------------- READ OPERATIONS --------------------

        public async Task<List<QualityCheckDto>> GetAllQualityChecksAsync()
        {
            var qualityChecks = await _qcRepo.GetAllQualityChecksAsync();

            return qualityChecks.Select(q => new QualityCheckDto
            {
                QcId = q.QcId,
                WorkOrderId = q.WorkOrderId,
                ProductId = q.ProductId,
                InspectionDate = q.InspectionDate,
                TotalQty = q.TotalQty,
                AcceptedQty = q.AcceptedQty,
                RejectedQty = q.RejectedQty,
                SuccessRate = q.SuccessRate,
                Result = q.Result,
                Remarks = q.Remarks,
                CreatedAt = q.CreatedAt
            }).ToList();
        }

        public async Task<QualityCheckDto?> GetQualityCheckByIdAsync(string qcId)
        {
            var qualityCheck = await _qcRepo.GetQualityCheckByIdAsync(qcId);
            if (qualityCheck == null) return null;

            return new QualityCheckDto
            {
                QcId = qualityCheck.QcId,
                WorkOrderId = qualityCheck.WorkOrderId,
                ProductId = qualityCheck.ProductId,
                InspectionDate = qualityCheck.InspectionDate,
                TotalQty = qualityCheck.TotalQty,
                AcceptedQty = qualityCheck.AcceptedQty,
                RejectedQty = qualityCheck.RejectedQty,
                SuccessRate = qualityCheck.SuccessRate,
                Result = qualityCheck.Result,
                Remarks = qualityCheck.Remarks,
                CreatedAt = qualityCheck.CreatedAt
            };
        }

        public async Task<QualityCheckDto?> GetQualityCheckByWorkOrderIdAsync(string workOrderId)
        {
            var qualityCheck = await _qcRepo.GetQualityCheckByWorkOrderIdAsync(workOrderId);
            if (qualityCheck == null) return null;

            return new QualityCheckDto
            {
                QcId = qualityCheck.QcId,
                WorkOrderId = qualityCheck.WorkOrderId,
                ProductId = qualityCheck.ProductId,
                InspectionDate = qualityCheck.InspectionDate,
                TotalQty = qualityCheck.TotalQty,
                AcceptedQty = qualityCheck.AcceptedQty,
                RejectedQty = qualityCheck.RejectedQty,
                SuccessRate = qualityCheck.SuccessRate,
                Result = qualityCheck.Result,
                Remarks = qualityCheck.Remarks,
                CreatedAt = qualityCheck.CreatedAt
            };
        }

        public async Task<List<QualityCheckDto>> GetQualityChecksByResultAsync(string result)
        {
            var qualityChecks = await _qcRepo.GetQualityChecksByResultAsync(result);

            return qualityChecks.Select(q => new QualityCheckDto
            {
                QcId = q.QcId,
                WorkOrderId = q.WorkOrderId,
                ProductId = q.ProductId,
                InspectionDate = q.InspectionDate,
                TotalQty = q.TotalQty,
                AcceptedQty = q.AcceptedQty,
                RejectedQty = q.RejectedQty,
                SuccessRate = q.SuccessRate,
                Result = q.Result,
                Remarks = q.Remarks,
                CreatedAt = q.CreatedAt
            }).ToList();
        }

        // -------------------- CREATE OPERATION --------------------

        public async Task<QualityCheckDto> CreateQualityCheckAsync(CreateQualityCheckDto dto)
        {
            // Verify work order exists and is COMPLETED
            var workOrder = await _workOrderRepo.GetWorkOrderByIdAsync(dto.WorkOrderId);
            if (workOrder == null)
                throw new KeyNotFoundException($"Work Order {dto.WorkOrderId} not found");

            if (workOrder.Status != "COMPLETED")
                throw new InvalidOperationException("Only COMPLETED work orders can be inspected");

            // Check if quality check already exists for this work order
            var existing = await _qcRepo.GetQualityCheckByWorkOrderIdAsync(dto.WorkOrderId);
            if (existing != null)
                throw new InvalidOperationException("Quality check already exists for this work order");

            // Validate accepted quantity
            if (dto.AcceptedQty > workOrder.Quantity)
                throw new InvalidOperationException("Accepted quantity cannot exceed total quantity");

            // Calculate metrics
            int totalQty = workOrder.Quantity;
            int acceptedQty = dto.AcceptedQty;
            int rejectedQty = totalQty - acceptedQty;
            int successRate = totalQty > 0 ? (int)Math.Round((double)acceptedQty / totalQty * 100) : 0;
            string result = successRate >= 90 ? "PASS" : "FAIL";

            // Generate QC ID
            string qcId = $"QC-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

            // Create quality check
            var qualityCheck = new QualityCheck
            {
                QcId = qcId,
                WorkOrderId = dto.WorkOrderId,
                ProductId = workOrder.ProductId,
                InspectionDate = DateTime.UtcNow.Date,
                TotalQty = totalQty,
                AcceptedQty = acceptedQty,
                RejectedQty = rejectedQty,
                SuccessRate = successRate,
                Result = result,
                Remarks = dto.Remarks,
                CreatedAt = DateTime.UtcNow
            };

            var saved = await _qcRepo.CreateQualityCheckAsync(qualityCheck);

            // Update work order status to QUALITY_DONE
            workOrder.Status = "QUALITY_DONE";
            await _workOrderRepo.UpdateWorkOrderAsync(workOrder);

            return new QualityCheckDto
            {
                QcId = saved.QcId,
                WorkOrderId = saved.WorkOrderId,
                ProductId = saved.ProductId,
                InspectionDate = saved.InspectionDate,
                TotalQty = saved.TotalQty,
                AcceptedQty = saved.AcceptedQty,
                RejectedQty = saved.RejectedQty,
                SuccessRate = saved.SuccessRate,
                Result = saved.Result,
                Remarks = saved.Remarks,
                CreatedAt = saved.CreatedAt
            };
        }

        // -------------------- DELETE OPERATION --------------------

        public async Task<bool> DeleteQualityCheckAsync(string qcId)
        {
            return await _qcRepo.DeleteQualityCheckAsync(qcId);
        }
    }
}