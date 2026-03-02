// ============================================================
// AUTH SERVICE (BUSINESS LOGIC LAYER)
// ============================================================
//
// This class contains ALL authentication rules.
//
// Responsibilities:
// ✔ Validate login credentials
// ✔ Hash passwords securely
// ✔ Generate JWT tokens
// ✔ Enforce role validation
//
// IMPORTANT:
// ❌ This class does NOT talk directly to the database
// ✅ It uses IAuthRepository (Repository Layer)
//
// Architecture Rule:
// Controller → Service → Repository → Database
// ============================================================



// ============================================================
// USING STATEMENTS
// ============================================================

// Required for JWT token creation & validation
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

// Used to store user information inside JWT
using System.Security.Claims;

// Converts secret key string → byte[]
using System.Text;

// DTOs for request/response
using ManuBackend.DTOs;

// Database entity
using ManuBackend.Models;

// Repository abstraction
using ManuBackend.Repository;



// ============================================================
// NAMESPACE
// ============================================================

namespace ManuBackend.Services
{
    // AuthService implements IAuthService interface
    // This ensures loose coupling and testability
    public class AuthService : IAuthService
    {
        // =====================================================
        // PRIVATE FIELDS
        // =====================================================

        // Repository used for database access
        private readonly IAuthRepository _repo;

        // IConfiguration is used to read appsettings.json values
        // Example: JWT Key, Issuer, Audience
        private readonly IConfiguration _config;



        // =====================================================
        // CONSTRUCTOR (DEPENDENCY INJECTION)
        // =====================================================

        // ASP.NET Core automatically injects:
        // - IAuthRepository
        // - IConfiguration
        //
        // Because both are registered in Program.cs
        public AuthService(IAuthRepository repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }



        // =====================================================
        // LOGIN LOGIC
        // =====================================================

        // This method:
        // - Validates email & password
        // - Generates JWT token on success
        //
        // async → non-blocking
        // Task<AuthResponseDto> → returns result later
        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            // STEP 1: Fetch user from database using email
            //
            // This goes to Repository → Database
            var user = await _repo.GetUserByEmailAsync(dto.Email);

            // STEP 2: If user does not exist
            // Do NOT reveal whether email or password is wrong
            // (Security best practice)
            if (user == null)
                throw new UnauthorizedAccessException("Invalid email or password");



            // STEP 3: Verify password using BCrypt
            //
            // dto.Password        → plain text (from user)
            // user.PasswordHash  → hashed password (from DB)
            //
            // BCrypt handles:
            // ✔ Salt
            // ✔ Hash comparison
            bool isPasswordValid =
                BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!isPasswordValid)
                throw new UnauthorizedAccessException("Invalid email or password");



            // STEP 4: Login successful → Generate JWT token
            return new AuthResponseDto
            {
                Token = GenerateJwtToken(user),
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };
        }



        // =====================================================
        // SIGNUP LOGIC
        // =====================================================

        // This method:
        // - Validates role
        // - Hashes password
        // - Creates user
        // - Returns JWT token
        public async Task<AuthResponseDto> SignupAsync(SignupDto dto)
        {
            // STEP 1: Check if email already exists
            bool emailTaken = await _repo.EmailExistsAsync(dto.Email);

            if (emailTaken)
                throw new InvalidOperationException("Email is already registered");



            // STEP 2: Validate allowed roles
            //
            // This prevents:
            // ❌ Users registering as admin illegally
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



            // STEP 3: Hash password using BCrypt
            //
            // Why hashing?
            // ❌ Never store plain passwords
            // ✔ Hashing is one-way (cannot be reversed)
            string hashedPassword =
                BCrypt.Net.BCrypt.HashPassword(dto.Password);



            // STEP 4: Create User entity
            var newUser = new User
            {
                Name = dto.Name,
                Email = dto.Email.ToLower(), // Normalization
                PasswordHash = hashedPassword,
                Role = dto.Role,
                CreatedAt = DateTime.UtcNow
            };



            // STEP 5: Save user to database
            var savedUser = await _repo.CreateUserAsync(newUser);



            // STEP 6: Generate JWT and return response
            return new AuthResponseDto
            {
                Token = GenerateJwtToken(savedUser),
                Name = savedUser.Name,
                Email = savedUser.Email,
                Role = savedUser.Role
            };
        }



        // =====================================================
        // JWT TOKEN GENERATION
        // =====================================================

        // JWT = JSON Web Token
        //
        // Think of JWT like a DIGITAL ID CARD.
        // Server signs it → Client carries it → Server verifies it
        private string GenerateJwtToken(User user)
        {
            // =================================================
            // CLAIMS (DATA STORED INSIDE TOKEN)
            // =================================================

            // Claims are user information embedded inside JWT
            var claims = new[]
            {
                // Unique user identifier
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),

                // Email
                new Claim(ClaimTypes.Email, user.Email),

                // Display name
                new Claim(ClaimTypes.Name, user.Name),

                // Role (used by [Authorize(Roles = "...")])
                new Claim(ClaimTypes.Role, user.Role)
            };



            // =================================================
            // SECRET KEY
            // =================================================

            // Read secret key from appsettings.json
            //
            // This key is used to SIGN the token
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
            );



            // =================================================
            // SIGNING CREDENTIALS
            // =================================================

            // HmacSha256 = secure hashing algorithm
            var credentials =
                new SigningCredentials(key, SecurityAlgorithms.HmacSha256);



            // =================================================
            // TOKEN CREATION
            // =================================================

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],     // Who created token
                audience: _config["Jwt:Audience"], // Who can use token
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8), // Token validity
                signingCredentials: credentials
            );



            // =================================================
            // TOKEN SERIALIZATION
            // =================================================

            // Convert token object → string
            // This string is sent to client
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}