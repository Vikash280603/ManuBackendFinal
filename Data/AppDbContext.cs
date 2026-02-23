// This using statement allows us to use Entity Framework Core classes
// like DbContext, DbSet, ModelBuilder, etc.
// Without this, EF Core-related classes will not be recognized.
// This gives access to our Models folder (User, Product, BOM classes)
// So we can use those classes inside this file.
using ManuBackend.Models;
using Microsoft.EntityFrameworkCore;

// Namespace = logical grouping of related classes
// Here, this file belongs to the "Data" layer of the project.
// Usually Data folder contains DbContext and database-related files.
namespace ManuBackend.Data
{
    // AppDbContext is our main bridge between the application and the database.
    // DbContext is a built-in EF Core class.
    // By inheriting from DbContext, we get all database functionality.
    public class AppDbContext : DbContext
    {
        // ---------------- CONSTRUCTOR ----------------
        // It is used to initialize the object.

        // DbContextOptions<AppDbContext> contains:
        // - Connection string
        // - Database provider (SQL Server, MySQL, etc.)
        // - Other EF configuration settings

        // These options are passed from Program.cs using Dependency Injection.
        // "base(options)" means we are passing these options to the parent class (DbContext).
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            // Nothing written here because all configuration
            // is already handled by the base DbContext.
        }


        // ---------------- DbSet PROPERTIES ----------------

        // DbSet<User> represents a table in the database.
        // "User" = Model class
        // "Users" = Table name in the database

        // EF Core automatically creates a table named "Users"
        // when we run migrations.

        // This property allows us to:
        // - Add users
        // - Update users
        // - Delete users
        // - Query users
        public DbSet<User> Users { get; set; }


        // This represents the "Products" table in the database.
        // Each Product object becomes a row in the Products table.
        public DbSet<Product> Products { get; set; }


        // This represents the "BOMs" table in the database.
        // BOM = Bill Of Materials
        // Each BOM object becomes a row in the BOMs table.
        public DbSet<BOM> BOMs { get; set; }

     
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<InventoryMaterial> InventoryMaterials { get; set; }

        //Table for Work Orders

        public DbSet<WorkOrder> WorkOrders { get; set; }


        // ---------------- MODEL CONFIGURATION ----------------

        // OnModelCreating is a method from DbContext.
        // It is used to configure database rules and relationships.

        // "override" means we are modifying behavior of a method
        // that already exists in the parent class (DbContext).
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Always call base method first
            // so default configurations are not lost.
            base.OnModelCreating(modelBuilder);

            // modelBuilder is used to configure entities (tables).

            // Here we are saying:
            // For the User entity,
            // create an INDEX on the Email column.

            // What is an index?
            // It makes searching faster and can enforce uniqueness.

            modelBuilder.Entity<User>()  // Select the User entity
                .HasIndex(u => u.Email)  // Create index on Email property
                .IsUnique();             // Make it UNIQUE (no duplicates allowed)
                                         // -------------------- INVENTORY CONFIGURATION --------------------  
            modelBuilder.Entity<Inventory>(entity =>
            {
                entity.HasKey(i => i.InventoryId);

                entity.Property(i => i.Location)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasMany(i => i.Materials)
                    .WithOne(m => m.Inventory)
                    .HasForeignKey(m => m.InventoryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<InventoryMaterial>(entity =>
            {
                entity.HasKey(m => m.Id);

                entity.Property(m => m.MaterialName)
                    .IsRequired()
                    .HasMaxLength(200);
            });
            // This ensures:
            // ❌ Two users cannot register with the same email.
            // If they try, database will throw an error.


            // -------------------- WORK ORDER CONFIGURATION --------------------  
            modelBuilder.Entity<WorkOrder>(entity =>
            {
                entity.HasKey(w => w.WorkOrderId); // Primary key
                entity.Property(w=>w.Status)
                .IsRequired()
                .HasMaxLength(50);
            });


            // NOTE:
            // Previously seed data (default users) was added here.
            // But it was removed because:
            // - Password hashing generates different values each time
            // - This caused migration inconsistencies
            // Now users should be added through API registration.
        }
    }
}
