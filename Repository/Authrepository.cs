// ============================================================
// AUTH REPOSITORY (DATA ACCESS LAYER)
// ============================================================
//
// This class contains ACTUAL database code.
// It talks directly to the database using Entity Framework Core.
//
// Responsibilities:
// ✔ Fetch users from database
// ✔ Insert new users
// ✔ Check email existence
//
// IMPORTANT:
// ❌ No business logic here
// ❌ No password validation
// ❌ No JWT generation
//
// Architecture:
// Controller → Service → Repository → Database
// ============================================================



// ============================================================
// USING STATEMENTS
// ============================================================

// Required for EF Core async database operations
using Microsoft.EntityFrameworkCore;

// Gives access to AppDbContext
using ManuBackend.Data;

// Database entity (User)
using ManuBackend.Models;



// ============================================================
// NAMESPACE
// ============================================================

namespace ManuBackend.Repository
{
    // =========================================================
    // AUTH REPOSITORY IMPLEMENTATION
    // =========================================================

    // This class IMPLEMENTS IAuthRepository
    //
    // Because of this:
    // ✔ It must implement all interface methods
    // ✔ Compiler enforces correctness
    public class AuthRepository : IAuthRepository
    {
        // =====================================================
        // PRIVATE FIELDS
        // =====================================================

        // AppDbContext represents:
        // - Database connection
        // - EF Core change tracking
        // - Transaction scope
        //
        // Injected by Dependency Injection
        private readonly AppDbContext _context;



        // =====================================================
        // CONSTRUCTOR (DEPENDENCY INJECTION)
        // =====================================================

        // ASP.NET Core automatically provides AppDbContext
        // because it was registered in Program.cs
        public AuthRepository(AppDbContext context)
        {
            _context = context;
        }



        // =====================================================
        // GET USER BY EMAIL
        // =====================================================

        // Used during LOGIN
        //
        // Returns:
        // - User object if found
        // - null if no user exists
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            // LINQ Query:
            // - Translated into SQL by EF Core
            //
            // FirstOrDefaultAsync:
            // - Returns first matching record
            // - Returns null if nothing matches
            //
            // ToLower():
            // - Makes comparison case-insensitive
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }



        // =====================================================
        // CREATE NEW USER
        // =====================================================

        // Used during SIGNUP
        //
        // Saves a new user record to database
        public async Task<User> CreateUserAsync(User user)
        {
            // Step 1:
            // Add entity to EF Core Change Tracker
            //
            // At this point:
            // ❌ No SQL is executed yet
            _context.Users.Add(user);



            // Step 2:
            // SaveChangesAsync():
            // - Generates SQL INSERT command
            // - Executes it against database
            // - Updates user.Id with generated value
            await _context.SaveChangesAsync();



            // Return saved entity
            return user;
        }



        // =====================================================
        // CHECK IF EMAIL EXISTS
        // =====================================================

        // Used during SIGNUP
        //
        // Returns:
        // - true  → email already exists
        // - false → email is available
        public async Task<bool> EmailExistsAsync(string email)
        {
            // AnyAsync():
            // - Stops searching after first match
            // - Very efficient for existence checks
            return await _context.Users
                .AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }
    }
}