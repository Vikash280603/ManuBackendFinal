// BOM = Bill of Materials  
// This entity represents the materials needed to make a product  
// Each row links to a Product via foreign key  

using ManuBackend.Models;

namespace ManuBackend.Models
{
    public class BOM
    {
        // Primary key - unique identifier for each BOM entry  
        public int BOMID { get; set; }

        // Foreign key - links this BOM row to a Product  
        // This is the Product.Id  
        public int ProductId { get; set; }

        // Name of the material (e.g., "Steel Head", "Wooden Handle")  
        public string MaterialName { get; set; } = string.Empty;

        // Quantity of this material needed  
        public int Quantity { get; set; }

        // Timestamp when BOM entry was created  
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property - EF Core uses this to link back to Product  
        // Each BOM belongs to ONE product  
        public virtual Product Product { get; set; } = null!;
    }
}