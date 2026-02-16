// DTO for updating an existing product  
// All fields are optional (user can update just name, or just status, etc.)  

namespace ManuBackend.DTOs
{
    public class UpdateProductDto
    {
        public string? Name { get; set; }
        public string? Category { get; set; }
        public string? Status { get; set; }
    }
}