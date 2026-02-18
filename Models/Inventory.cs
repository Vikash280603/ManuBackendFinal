// Inventory entity - represents inventory for a product at a location  
// One product can have inventory at multiple locations  
namespace ManuBackend.Models
{
    public class Inventory
    {
        // Primary key - auto-incremented  
        public int InventoryId { get; set; }

        // Foreign key - links to Product  
        public int ProductId { get; set; }

        // Location where this inventory is stored  
        // Examples: "Chennai", "Coimbatore", "Bangalore", "Hyderabad"  
        public string Location { get; set; } = string.Empty;

        // Timestamp when inventory record was created  
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property - one inventory has many materials  
        public virtual ICollection<InventoryMaterial> Materials { get; set; } = new List<InventoryMaterial>();
    }
}
