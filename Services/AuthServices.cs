// The AuthService handles all the business logic:  
// - Hashing passwords with BCrypt  
// - Validating credentials  
// - Creating JWT tokens  

//using ManuBackend.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ManuBackend.DTOs;
using ManuBackend.Models;
using ManuBackend.Repository;

namespace ManuBackend.Services

{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config; // Reads values from appsettings.json  

        public AuthService(IAuthRepository repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }

        // -------------------- LOGIN --------------------  
        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            // Step 1: Find the user by email  
            var user = await _repo.GetUserByEmailAsync(dto.Email);

            // Step 2: If user not found, throw an error  
            if (user == null)
                throw new UnauthorizedAccessException("Invalid email or password");

            // Step 3: Verify the password using BCrypt  
            // BCrypt.Verify compares plain text with the stored hash  
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!isPasswordValid)
                throw new UnauthorizedAccessException("Invalid email or password");

            // Step 4: Create and return a JWT token  
            return new AuthResponseDto
            {
                Token = GenerateJwtToken(user),
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };
        }

        // -------------------- SIGNUP --------------------  
        public async Task<AuthResponseDto> SignupAsync(SignupDto dto)
        {
            // Step 1: Check if email is already used  
            bool emailTaken = await _repo.EmailExistsAsync(dto.Email);
            if (emailTaken)
                throw new InvalidOperationException("Email is already registered");

            // Step 2: Validate the role is one of our allowed roles  
            var allowedRoles = new[]
            {
                "product_bom_manager",
                "inventory_manager",
                "qc_manager",
                "production_scheduler",
                "admin"
            };

            if (!allowedRoles.Contains(dto.Role))
                throw new InvalidOperationException("Invalid role selected");

            // Step 3: Hash the password before saving  
            // BCrypt.HashPassword creates a secure one-way hash  
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // Step 4: Create the new user object  
            var newUser = new User
            {
                Name = dto.Name,
                Email = dto.Email.ToLower(),  // Always store emails in lowercase  
                PasswordHash = hashedPassword,
                Role = dto.Role,
                CreatedAt = DateTime.UtcNow
            };

            // Step 5: Save to database  
            var savedUser = await _repo.CreateUserAsync(newUser);

            // Step 6: Return JWT token  
            return new AuthResponseDto
            {
                Token = GenerateJwtToken(savedUser),
                Name = savedUser.Name,
                Email = savedUser.Email,
                Role = savedUser.Role
            };
        }

        // -------------------- JWT TOKEN GENERATOR --------------------  
        // A JWT token is like an ID card - it contains user info and is digitally signed  
        private string GenerateJwtToken(User user)
        {
            // Claims = pieces of information stored inside the token  
            // The client can read these (but not modify them - they are signed)  
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role)
            };

            // The secret key - must match what's in appsettings.json  
            // This key is used to SIGN the token so no one can fake it  
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
            );

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Build the actual JWT token  
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],       // Who created the token  
                audience: _config["Jwt:Audience"],   // Who the token is for  
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8), // Token expires after 8 hours  
                signingCredentials: credentials
            );

            // Serialize the token to a string (this is what we send to the client)  
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}