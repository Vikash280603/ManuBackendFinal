// This file defines the Product "entity" - maps to "Products" table in database  
// Each instance represents ONE row in the Products table  

namespace ManuBackend.Models
{
    public class Product
    {
        // Primary key - auto-incremented by database  
        public int Id { get; set; }

        // Product name (e.g., "Hammer", "Wrench")  
        public string Name { get; set; } = string.Empty;

        // Category for grouping products (e.g., "Mechanical", "Electrical")  
        public string Category { get; set; } = string.Empty;

        // Status indicates if product is active or discontinued  
        // Values: "ACTIVE" or "DISCONTINUED"  
        public string Status { get; set; } = "ACTIVE";

        // Timestamp when product was created  
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property - EF Core uses this to link to BOM items  
        // One product can have many BOM items  
        public virtual ICollection<BOM> BOMs { get; set; } = new List<BOM>();
    }
}