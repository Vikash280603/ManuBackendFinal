using System.ComponentModel.DataAnnotations;

namespace ManuBackend.DTOs
{
    public class CreateWorkOrderDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        public DateTime? ScheduledDate { get; set; }

    }
}
