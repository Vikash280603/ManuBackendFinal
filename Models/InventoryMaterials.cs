namespace ManuBackend.Models
{
    public class InventoryMaterial
    {
        public int Id { get; set; }
        public int InventoryId { get; set; }
        public string MaterialName { get; set; } = string.Empty;
        public int AvailableQty { get; set; }
        public int ThresholdQty { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual Inventory Inventory { get; set; } = null!;
    }
}