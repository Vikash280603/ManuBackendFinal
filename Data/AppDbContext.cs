// ============================================================
// USING STATEMENTS
// ============================================================

// Gives access to model classes (User, Product, BOM, etc.)
using ManuBackend.Models;

// Required for Entity Framework Core (DbContext, DbSet, ModelBuilder)
using Microsoft.EntityFrameworkCore;



// ============================================================
// NAMESPACE
// ============================================================

// Groups database-related classes together
// Usually contains DbContext and migration files
namespace ManuBackend.Data
{
    // ========================================================
    // DATABASE CONTEXT
    // ========================================================

    // AppDbContext is the main connection between:
    // - Your C# classes (Models)
    // - Your database tables
    //
    // DbContext is provided by Entity Framework Core.
    // By inheriting from DbContext, we get:
    // - Database connections
    // - CRUD operations
    // - Change tracking
    // - Migrations support
    public class AppDbContext : DbContext
    {
        // ====================================================
        // CONSTRUCTOR
        // ====================================================

        // DbContextOptions<AppDbContext> contains:
        // - Connection string
        // - Database provider (SQL Server)
        // - EF Core configuration
        //
        // These options are injected from Program.cs
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
            // No code needed here
            // Base DbContext handles everything
        }



        // ====================================================
        // DBSET PROPERTIES (DATABASE TABLES)
        // ====================================================

        // DbSet<User> → Users table
        // Each User object = one row in database
        public DbSet<User> Users { get; set; }

        // DbSet<Product> → Products table
        public DbSet<Product> Products { get; set; }

        // DbSet<BOM> → BOMs table (Bill Of Materials)
        public DbSet<BOM> BOMs { get; set; }

        // Inventory tables
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<InventoryMaterial> InventoryMaterials { get; set; }

        // Work Orders table
        public DbSet<WorkOrder> WorkOrders { get; set; }

        // Quality Check table
        public DbSet<QualityCheck> QualityChecks { get; set; }



        // ====================================================
        // MODEL CONFIGURATION (FLUENT API)
        // ====================================================

        // This method is called when EF Core builds the database model
        // Used to configure:
        // - Keys
        // - Indexes
        // - Relationships
        // - Constraints
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Always call base method
            base.OnModelCreating(modelBuilder);



            // ====================================================
            // USER CONFIGURATION
            // ====================================================

            // Create UNIQUE index on Email column
            // This ensures:
            // - No two users can have the same email
            // - Faster search by email
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();



            // ====================================================
            // INVENTORY CONFIGURATION
            // ====================================================

            modelBuilder.Entity<Inventory>(entity =>
            {
                // Primary key
                entity.HasKey(i => i.InventoryId);

                // Location is mandatory and limited to 100 characters
                entity.Property(i => i.Location)
                    .IsRequired()
                    .HasMaxLength(100);

                // One Inventory → Many Materials
                entity.HasMany(i => i.Materials)  //  define navigations (how entities reference each other in C#).
                    .WithOne(m => m.Inventory)  // define the inverse navigation.   
                    .HasForeignKey(m => m.InventoryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<InventoryMaterial>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.MaterialName).IsRequired().HasMaxLength(200);

                // ✅ ADD: Unique constraint (prevent duplicate material names in same inventory)
                entity.HasIndex(m => new { m.InventoryId, m.MaterialName })
                      .IsUnique()
                      .HasDatabaseName("IX_Inventory_Material_Unique");
            });



            // ====================================================
            // WORK ORDER CONFIGURATION
            // ====================================================

            modelBuilder.Entity<WorkOrder>(entity =>
            {
                // Primary key
                entity.HasKey(w => w.WorkOrderId);

                // Status is required (Planned, InProgress, Completed, etc.)
                entity.Property(w => w.Status)
                    .IsRequired()
                    .HasMaxLength(50);
            });



            // ====================================================
            // QUALITY CHECK CONFIGURATION
            // ====================================================

            modelBuilder.Entity<QualityCheck>(entity =>
            {
                // Primary key
                entity.HasKey(q => q.QcId);

                // Result must be provided (Pass / Fail)
                entity.Property(q => q.Result)
                    .IsRequired()
                    .HasMaxLength(10);

                // Optional remarks
                entity.Property(q => q.Remarks)
                    .HasMaxLength(1000);
            });
        }
    }
}