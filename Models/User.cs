// This file defines the User "entity" - it maps directly to a database table called "Users"  
// Think of this class as a blueprint for one row in the Users table  

namespace ManuBackend.Models

{
    public class User
    {
        // Primary key - EF Core will auto-increment this  
        public int Id { get; set; }

        // User's display name (e.g. "John Doe")  
        public string Name { get; set; } = string.Empty;

        // Email used for login - must be unique per user  
        public string Email { get; set; } = string.Empty;

        // We will NEVER store plain text passwords  
        // BCrypt will hash it before saving  
        public string PasswordHash { get; set; } = string.Empty;

        // Role controls what pages the user can access  
        // Examples: "admin", "inventory_manager", "qc_manager"  
        public string Role { get; set; } = string.Empty;

        // When was this account created  
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}