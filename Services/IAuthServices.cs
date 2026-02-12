// Interface for the Auth Service  
// The service handles business logic (hashing, JWT creation, validation)  

using ManuBackend.DTOs;


namespace ManuBackend.Services
{
    public interface IAuthService
    {
        // Process login: validate credentials, return token  
        Task<AuthResponseDto> LoginAsync(LoginDto dto);

        // Process signup: validate, hash password, save user, return token  
        Task<AuthResponseDto> SignupAsync(SignupDto dto);
    }
}