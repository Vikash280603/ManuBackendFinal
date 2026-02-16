// DTO for creating a new product  
// Client sends this when adding a product  

using System.ComponentModel.DataAnnotations;

namespace ManuBackend.DTOs
{
    public class CreateProductDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Category { get; set; } = string.Empty;

        // Status defaults to "ACTIVE" if not provided  
        public string Status { get; set; } = "ACTIVE";
    }
}