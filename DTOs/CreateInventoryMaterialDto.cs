// DTO for creating new inventory record  
using System.ComponentModel.DataAnnotations;
namespace ManuBackend.DTOs
{
    public class CreateInventoryMaterialDto
    {
        [Required]
        public string MaterialName { get; set; } = string.Empty;

        [Required]
        [Range(0, int.MaxValue)]
        public int AvailableQty { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int ThresholdQty { get; set; }
    }
}
