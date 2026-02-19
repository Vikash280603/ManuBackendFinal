using ManuBackend.Models;

namespace ManuBackend.Models
{
    public class Inventory
    {
        public int InventoryId { get; set; }
        public int ProductId { get; set; }
        public string Location { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<InventoryMaterial> Materials { get; set; } = new List<InventoryMaterial>();
    }
}