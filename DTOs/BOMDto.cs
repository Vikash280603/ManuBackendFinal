// DTO for BOM item (used in responses)  

namespace ManuBackend.DTOs
{
    public class BOMDto
    {
        public int BOMID { get; set; }
        public int ProductId { get; set; }
        public string MaterialName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}