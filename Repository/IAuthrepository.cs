// This is the INTERFACE for the authentication repository
// An interface acts like a contract
// Any class that implements IAuthRepository MUST implement these methods
// This helps with clean architecture, testing, and flexibility

using ManuBackend.Models;

namespace ManuBackend.Repository
{
    // IAuthRepository defines the database operations related to authentication
    public interface IAuthRepository
    {
        // -------------------- Get User By Email --------------------
        // Finds a user using their email address
        // Returns:
        // - User object if found
        // - null if no user exists with that email
        Task<User?> GetUserByEmailAsync(string email);

        // -------------------- Create New User --------------------
        // Saves a new user record to the database
        // Returns the created User object
        Task<User> CreateUserAsync(User user);

        // -------------------- Check If Email Exists --------------------
        // Checks whether an email is already registered
        // Returns:
        // - true if email exists
        // - false if email is available
        Task<bool> EmailExistsAsync(string email);
    }
}
