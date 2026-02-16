// DTO returned when fetching product details  
// Includes the product info + its BOM items  

namespace ManuBackend.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; } 
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // List of BOM items for this product  
        public List<BOMDto> BOMs { get; set; } = new();
    }
}