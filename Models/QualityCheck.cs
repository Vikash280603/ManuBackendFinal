
namespace ManuBackend.Models
{
    public class QualityCheck
    {
        // Primary key - auto-generated string ID
        public string QcId { get; set; } = string.Empty;

        // Foreign key to Work Order
        public string WorkOrderId { get; set; } = string.Empty;

        // Foreign key to Product
        public int ProductId { get; set; }

        // Date of inspection
        public DateTime InspectionDate { get; set; }

        // Total quantity produced
        public int TotalQty { get; set; }

        // Quantity that passed inspection
        public int AcceptedQty { get; set; }

        // Quantity that failed inspection
        public int RejectedQty { get; set; }

        // Success rate percentage (0-100)
        public int SuccessRate { get; set; }

        // Final result: "PASS" or "FAIL"
        public string Result { get; set; } = string.Empty;

        // Inspector's notes
        public string? Remarks { get; set; }

        // When record was created
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}