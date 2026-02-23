using ManuBackend.Models;
using ManuBackend.Models;

namespace ManuBackend.Repository
{
    public interface IQualityCheckRepository
    {
        Task<List<QualityCheck>> GetAllQualityChecksAsync();
        Task<QualityCheck?> GetQualityCheckByIdAsync(string qcId);
        Task<QualityCheck?> GetQualityCheckByWorkOrderIdAsync(string workOrderId);
        Task<List<QualityCheck>> GetQualityChecksByResultAsync(string result);
        Task<QualityCheck> CreateQualityCheckAsync(QualityCheck qualityCheck);
        Task<bool> DeleteQualityCheckAsync(string qcId);
    }
}