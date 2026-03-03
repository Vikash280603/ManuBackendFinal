// =============================================================
// QUALITY CHECK SERVICE
// =============================================================
//
// Service layer responsibilities:
// ✅ Business rules & validations
// ✅ Coordinate multiple repositories
// ✅ DTO ↔ Entity mapping
//
// IMPORTANT INTERVIEW POINTS:
// ❌ No HTTP logic here
// ❌ No DbContext here
// ❌ No direct database calls
// ✅ Talks ONLY to repositories
// =============================================================

using ManuBackend.DTOs;
using ManuBackend.Models;
using ManuBackend.Repository;
using ManuBackend.Services;

namespace ManuBackend.Services
{
    public class QualityCheckService : IQualityCheckService
    {
        // Repository for QualityCheck entity
        private readonly IQualityCheckRepository _qcRepo;

        // Repository for WorkOrder entity
        // Used to validate work order state
        private readonly IWorkOrderRepository _workOrderRepo;

        // -------------------- CONSTRUCTOR --------------------
        // Repositories are injected using Dependency Injection (DI)
        public QualityCheckService(
            IQualityCheckRepository qcRepo,
            IWorkOrderRepository workOrderRepo)
        {
            _qcRepo = qcRepo;
            _workOrderRepo = workOrderRepo;
        }

        // =====================================================
        // READ OPERATIONS
        // =====================================================

        // -------------------- GET ALL QUALITY CHECKS --------------------
        public async Task<List<QualityCheckDto>> GetAllQualityChecksAsync()
        {
            // Fetch entities from repository
            var qualityChecks = await _qcRepo.GetAllQualityChecksAsync();

            // Map Entity → DTO
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

        // -------------------- GET QUALITY CHECK BY QC ID --------------------
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

        // -------------------- GET QUALITY CHECK BY WORK ORDER ID --------------------
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

        // -------------------- GET QUALITY CHECKS BY RESULT --------------------
        // Example result values: PASS / FAIL
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

        // =====================================================
        // CREATE OPERATION (BUSINESS LOGIC HEAVY)
        // =====================================================

        public async Task<QualityCheckDto> CreateQualityCheckAsync(CreateQualityCheckDto dto)
        {
            // -------------------- VALIDATION: WORK ORDER --------------------

            // Verify work order exists
            var workOrder = await _workOrderRepo.GetWorkOrderByIdAsync(dto.WorkOrderId);
            if (workOrder == null)
                throw new KeyNotFoundException($"Work Order {dto.WorkOrderId} not found");

            // Only COMPLETED work orders can be quality checked
            if (workOrder.Status != "COMPLETED")
                throw new InvalidOperationException("Only COMPLETED work orders can be inspected");

            // -------------------- VALIDATION: DUPLICATE QC --------------------

            // Ensure no duplicate quality check exists
            var existing = await _qcRepo.GetQualityCheckByWorkOrderIdAsync(dto.WorkOrderId);
            if (existing != null)
                throw new InvalidOperationException("Quality check already exists for this work order");

            // -------------------- VALIDATION: QUANTITY --------------------

            if (dto.AcceptedQty > workOrder.Quantity)
                throw new InvalidOperationException("Accepted quantity cannot exceed total quantity");

            // -------------------- CALCULATIONS --------------------

            int totalQty = workOrder.Quantity;
            int acceptedQty = dto.AcceptedQty;
            int rejectedQty = totalQty - acceptedQty;

            int successRate =
                totalQty > 0
                    ? (int)Math.Round((double)acceptedQty / totalQty * 100)
                    : 0;

            // Business rule:
            // >= 90% → PASS
            // < 90% → FAIL
            string result = successRate >= 90 ? "PASS" : "FAIL";

            // -------------------- QC ID GENERATION --------------------
            // Example: QC-1700000000000
            string qcId = $"QC-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";

            // -------------------- CREATE ENTITY --------------------
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

            // Save to database
            var saved = await _qcRepo.CreateQualityCheckAsync(qualityCheck);

            // -------------------- UPDATE WORK ORDER STATUS --------------------
            workOrder.Status = "QUALITY_DONE";
            await _workOrderRepo.UpdateWorkOrderAsync(workOrder);

            // -------------------- RETURN DTO --------------------
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

        // =====================================================
        // DELETE OPERATION
        // =====================================================

        public async Task<bool> DeleteQualityCheckAsync(string qcId)
        {
            return await _qcRepo.DeleteQualityCheckAsync(qcId);
        }
    }
}