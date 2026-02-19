// DTO for returning material data
namespace ManuBackend.DTOs
{ 
    public class InventoryMaterialDto
    {
        public int Id { get; set; }
        public int InventoryId { get; set; }
        public string MaterialName { get; set; } = string.Empty;
        public int AvailableQty { get; set; }
        public int ThresholdQty { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
