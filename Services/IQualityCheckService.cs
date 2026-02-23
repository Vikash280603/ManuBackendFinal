
using ManuBackend.DTOs;


namespace ManuBackend.Services
{
    public interface IQualityCheckService
    {
        Task<List<QualityCheckDto>> GetAllQualityChecksAsync();
        Task<QualityCheckDto?> GetQualityCheckByIdAsync(string qcId);
        Task<QualityCheckDto?> GetQualityCheckByWorkOrderIdAsync(string workOrderId);
        Task<List<QualityCheckDto>> GetQualityChecksByResultAsync(string result);
        Task<QualityCheckDto> CreateQualityCheckAsync(CreateQualityCheckDto dto);
        Task<bool> DeleteQualityCheckAsync(string qcId);
    }
}