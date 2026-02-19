using ManuBackend.DTOs;

namespace ManuBackend.DTOs
{
    public class InventoryDto
    {
        public int InventoryId { get; set; }
        public int ProductId { get; set; }
        public string Location { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<InventoryMaterialDto> Materials { get; set; } = new();
    }
}