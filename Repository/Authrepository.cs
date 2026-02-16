// This class implements IAuthRepository
// A Repository is responsible for handling database operations
// This class uses Entity Framework Core to communicate with the database

using Microsoft.EntityFrameworkCore;
using System;
using ManuBackend.Data;
using ManuBackend.Models;

namespace ManuBackend.Repository
{
    // AuthRepository contains the actual database logic for authentication-related operations
    public class AuthRepository : IAuthRepository
    {
        // AppDbContext represents the database session
        // It is injected via Dependency Injection
        private readonly AppDbContext _context;

        // Constructor - receives AppDbContext instance
        public AuthRepository(AppDbContext context)
        {
            _context = context;
        }

        // -------------------- Get User By Email --------------------
        // Searches for a user with the given email
        // Returns null if no user is found
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            // FirstOrDefaultAsync:
            // - Returns the first matching record
            // - Returns null if no match exists
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        // -------------------- Create New User --------------------
        // Adds a new user record to the database
        public async Task<User> CreateUserAsync(User user)
        {
            // Adds the user to EF Core's change tracker
            _context.Users.Add(user);

            // Saves changes to the database
            // This executes the INSERT SQL command
            await _context.SaveChangesAsync();

            return user;
        }

        // -------------------- Check If Email Exists --------------------
        // Returns true if a user with the email already exists
        public async Task<bool> EmailExistsAsync(string email)
        {
            // AnyAsync:
            // - Returns true if at least one record matches the condition
            return await _context.Users
                .AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }
    }
}
