// DTO for creating a new BOM entry  
// Client sends this when adding materials to a product  

using System.ComponentModel.DataAnnotations;

namespace ManuBackend.DTOs
{
    public class CreateBOMDto
    {
        [Required]
        public string MaterialName { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }
}