// This is the IMPLEMENTATION of IAuthRepository  
// It contains the actual database queries using EF Core  

using Microsoft.EntityFrameworkCore;
using System;
using ManuBackend.Data;
using ManuBackend.Models;

namespace ManuBackend.Repository
{
    public class AuthRepository : IAuthRepository
    {
        // We inject AppDbContext to talk to the database  
        private readonly AppDbContext _context;

        public AuthRepository(AppDbContext context)
        {
            _context = context;
        }

        // Find user by email - returns null if not found  
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            // .FirstOrDefaultAsync returns the first match, or null  
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        // Save a new user to the database  
        public async Task<User> CreateUserAsync(User user)
        {
            // Add the user to the "tracked" list  
            _context.Users.Add(user);

            // Actually save to database (INSERT SQL runs here)  
            await _context.SaveChangesAsync();

            return user;
        }

        // Check if email is already used  
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users
                .AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }
    }
}