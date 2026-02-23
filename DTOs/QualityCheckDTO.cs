
namespace ManuBackend.DTOs
{
    public class QualityCheckDto
    {
        public string QcId { get; set; } = string.Empty;
        public string WorkOrderId { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public DateTime InspectionDate { get; set; }
        public int TotalQty { get; set; }
        public int AcceptedQty { get; set; }
        public int RejectedQty { get; set; }
        public int SuccessRate { get; set; }
        public string Result { get; set; } = string.Empty;
        public string? Remarks { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}