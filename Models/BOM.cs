// -------------------------------------------------------------
// BOM ENTITY (MODEL)
// -------------------------------------------------------------
// BOM = Bill Of Materials
//
// This class represents a DATABASE TABLE.
//
// In EF Core:
// Class  → Table
// Property → Column
//
// Each BOM entry represents one material
// required to build a product.
// -------------------------------------------------------------

namespace ManuBackend.Models
{
    // Public class = accessible everywhere in project
    public class BOM
    {
        // ---------------------------------------------------------
        // PRIMARY KEY
        // ---------------------------------------------------------
        // Unique identifier for each row in BOM table.
        //
        // EF Core automatically understands:
        // Property named "BOMID" or "Id"
        // = Primary Key
        //
        // Database column:
        // BOMID (int, identity, primary key)
        // ---------------------------------------------------------
        public int BOMID { get; set; }



        // ---------------------------------------------------------
        // FOREIGN KEY
        // ---------------------------------------------------------
        // This connects BOM to Product table.
        //
        // It stores the Product's Id.
        //
        // Example:
        // If Product.Id = 5
        // Then ProductId = 5 here.
        //
        // This creates a relationship:
        // One Product → Many BOMs
        //
        // Database column:
        // ProductId (int, foreign key)
        // ---------------------------------------------------------
        public int ProductId { get; set; }



        // ---------------------------------------------------------
        // MATERIAL NAME
        // ---------------------------------------------------------
        // Name of raw material needed.
        //
        // string.Empty means:
        // Default value is empty string
        // (Avoids null reference issues)
        //
        // Database column:
        // MaterialName (nvarchar)
        // ---------------------------------------------------------
        public string MaterialName { get; set; } = string.Empty;



        // ---------------------------------------------------------
        // QUANTITY
        // ---------------------------------------------------------
        // How many units of this material are required.
        //
        // Example:
        // Hammer needs:
        // - 1 Steel Head
        // - 1 Wooden Handle
        //
        // Database column:
        // Quantity (int)
        // ---------------------------------------------------------
        public int Quantity { get; set; }



        // ---------------------------------------------------------
        // CREATED DATE
        // ---------------------------------------------------------
        // Stores when this BOM record was created.
        //
        // DateTime.UtcNow:
        // - Uses universal time
        // - Better for servers
        //
        // This value is automatically set
        // when new object is created.
        //
        // Database column:
        // CreatedAt (datetime)
        // ---------------------------------------------------------
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;



        // ---------------------------------------------------------
        // NAVIGATION PROPERTY
        // ---------------------------------------------------------
        // This allows accessing related Product object.
        //
        // ProductId = foreign key (integer)
        // Product = actual Product object
        //
        // Example:
        // bom.Product.Name
        //
        // virtual keyword:
        // Allows EF Core to use Lazy Loading
        //
        // null! means:
        // "We promise this won't be null"
        // (Avoids compiler warning)
        //
        // Relationship:
        // Each BOM belongs to ONE Product
        // ---------------------------------------------------------
        public virtual Product Product { get; set; } = null!;
    }
}
