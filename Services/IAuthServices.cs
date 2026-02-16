// This is the INTERFACE for the Authentication Service
// The service layer contains BUSINESS LOGIC (not database logic).
// It handles:
// - Password hashing
// - Credential validation
// - JWT token generation
//
// Any class implementing IAuthService MUST implement these methods.
// This keeps the architecture clean and easy to test.

using ManuBackend.DTOs;

namespace ManuBackend.Services
{
    public interface IAuthService
    {
        // -------------------- LOGIN --------------------
        // Validates user credentials (email + password)
        // If valid:
        //   - Generates a JWT token
        //   - Returns authentication response data
        // If invalid:
        //   - Throws UnauthorizedAccessException
        Task<AuthResponseDto> LoginAsync(LoginDto dto);

        // -------------------- SIGNUP --------------------
        // Validates new user data
        // - Checks if email already exists
        // - Validates role
        // - Hashes password securely
        // - Saves user to database
        // - Returns JWT token
        Task<AuthResponseDto> SignupAsync(SignupDto dto);
    }
}
