
using ManuBackend.Data;
using ManuBackend.Models;
using Microsoft.EntityFrameworkCore;


namespace ManuBackend.Repository
{
    public class QualityCheckRepository : IQualityCheckRepository
    {
        private readonly AppDbContext _context;

        public QualityCheckRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<QualityCheck>> GetAllQualityChecksAsync()
        {
            return await _context.QualityChecks
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();
        }

        public async Task<QualityCheck?> GetQualityCheckByIdAsync(string qcId)
        {
            return await _context.QualityChecks
                .FirstOrDefaultAsync(q => q.QcId == qcId);
        }

        public async Task<QualityCheck?> GetQualityCheckByWorkOrderIdAsync(string workOrderId)
        {
            return await _context.QualityChecks
                .FirstOrDefaultAsync(q => q.WorkOrderId == workOrderId);
        }

        public async Task<List<QualityCheck>> GetQualityChecksByResultAsync(string result)
        {
            return await _context.QualityChecks
                .Where(q => q.Result == result)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();
        }

        public async Task<QualityCheck> CreateQualityCheckAsync(QualityCheck qualityCheck)
        {
            _context.QualityChecks.Add(qualityCheck);
            await _context.SaveChangesAsync();
            return qualityCheck;
        }

        public async Task<bool> DeleteQualityCheckAsync(string qcId)
        {
            var qualityCheck = await _context.QualityChecks.FindAsync(qcId);
            if (qualityCheck == null) return false;

            _context.QualityChecks.Remove(qualityCheck);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}