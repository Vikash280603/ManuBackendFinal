// AuthService contains the BUSINESS LOGIC for authentication
// Responsibilities:
// - Validate user credentials
// - Hash passwords using BCrypt
// - Generate JWT tokens
// - Enforce role rules
//
// It does NOT directly talk to the database.
// Instead, it uses IAuthRepository (Repository Layer).

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
        // Repository for database operations
        private readonly IAuthRepository _repo;

        // IConfiguration allows reading values from appsettings.json
        private readonly IConfiguration _config;

        public AuthService(IAuthRepository repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }

        // ============================================================
        // LOGIN
        // ============================================================

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            // Step 1: Find user by email
            var user = await _repo.GetUserByEmailAsync(dto.Email);

            // Step 2: If user doesn't exist -> Unauthorized
            if (user == null)
                throw new UnauthorizedAccessException("Invalid email or password");

            // Step 3: Verify password using BCrypt
            // Compare plain password with stored hash
            bool isPasswordValid =
                BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!isPasswordValid)
                throw new UnauthorizedAccessException("Invalid email or password");

            // Step 4: Generate JWT token and return response
            return new AuthResponseDto
            {
                Token = GenerateJwtToken(user),
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };
        }

        // ============================================================
        // SIGNUP
        // ============================================================

        public async Task<AuthResponseDto> SignupAsync(SignupDto dto)
        {
            // Step 1: Check if email already exists
            bool emailTaken = await _repo.EmailExistsAsync(dto.Email);
            if (emailTaken)
                throw new InvalidOperationException("Email is already registered");

            // Step 2: Validate allowed roles
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

            // Step 3: Hash password securely using BCrypt
            string hashedPassword =
                BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // Step 4: Create new user object
            var newUser = new User
            {
                Name = dto.Name,
                Email = dto.Email.ToLower(), // Always store lowercase
                PasswordHash = hashedPassword,
                Role = dto.Role,
                CreatedAt = DateTime.UtcNow
            };

            // Step 5: Save user to database
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

        // ============================================================
        // JWT TOKEN GENERATOR
        // ============================================================

        // JWT (JSON Web Token) works like a digital ID card.
        // It contains user data (claims) and is digitally signed.
        private string GenerateJwtToken(User user)
        {
            // Claims = information stored inside the token
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role)
            };

            // Secret key from appsettings.json
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
            );

            // Signing credentials (HMAC SHA256 encryption)
            var credentials =
                new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Build token
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],     // Who created it
                audience: _config["Jwt:Audience"], // Who it’s for
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8), // Expiration time
                signingCredentials: credentials
            );

            // Convert token object to string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
