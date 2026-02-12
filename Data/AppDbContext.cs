using Microsoft.EntityFrameworkCore;
using ManuBackend.Models;

namespace ManuBackend.Data
{
    public class AppDbContext : DbContext
    {
        // DbContextOptions contains connection string and other EF settings  
        // It comes from dependency injection (set up in Program.cs)  
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSet = represents the "Users" table in the database  
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Make the Email column unique (no two users with same email)  
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // NOTE: Seed data has been removed to prevent non-deterministic 
            // hashing issues during migrations. Users should now be 
            // added via the API registration endpoint.
        }
    }
}