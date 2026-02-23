using System.ComponentModel.DataAnnotations;

namespace ManuBackend.DTOs
{
    public class CreateQualityCheckDto
    {
        [Required]
        public string WorkOrderId { get; set; } = string.Empty;

        [Required]
        [Range(0, int.MaxValue)]
        public int AcceptedQty { get; set; }

        public string? Remarks { get; set; }
    }
}