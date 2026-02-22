using System.ComponentModel.DataAnnotations;

namespace ManuBackend.DTOs
{
    public class UpdateWorkOrderDto
    {
        public string? Status { get; set; }

        [Range(1, int.MaxValue)]
        public int? Quantity { get; set; }

        public DateTime? ScheduledDate { get; set; }
    }
}
