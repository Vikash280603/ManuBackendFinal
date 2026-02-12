// This is the INTERFACE for the repository  
// An interface is like a contract: it says "whoever implements me MUST have these methods"  
// This makes code easier to test and swap out later  

using ManuBackend.Models;

namespace ManuBackend.Repository
{
    public interface IAuthRepository
    {
        // Find a user by their email address (returns null if not found)  
        Task<User?> GetUserByEmailAsync(string email);

        // Save a new user to the database  
        Task<User> CreateUserAsync(User user);

        // Check if an email is already taken  
        Task<bool> EmailExistsAsync(string email);
    }
}