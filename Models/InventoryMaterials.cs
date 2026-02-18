// InventoryMaterial entity - represents a material with its quantities  
// Each inventory record has multiple materials 
namespace ManuBackend.Models
{
    public class InventoryMaterials
    {
        // Primary key - auto-incremented  
        public int Id { get; set; }

        // Foreign key - links to Inventory  
        public int InventoryId { get; set; }

        // Name of the material (e.g., "Steel Head", "Wooden Handle")  
        public string MaterialName { get; set; } = string.Empty;

        // Current stock quantity available  
        public int AvailableQty { get; set; }

        // Minimum quantity threshold (reorder level)  
        // When AvailableQty < ThresholdQty, it's low stock  
        public int ThresholdQty { get; set; }

        // Timestamp  
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property - each material belongs to one inventory  
        public virtual Inventory Inventory { get; set; } = null!;
    }
}
