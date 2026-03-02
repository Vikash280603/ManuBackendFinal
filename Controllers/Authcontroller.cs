// =============================================================
// AUTH CONTROLLER
// =============================================================
//
// A Controller is the ENTRY POINT for HTTP requests.
// It does NOT contain business logic or database code.
//
// Responsibility:
// - Receive HTTP request
// - Validate input
// - Call Service layer
// - Return proper HTTP response
//
// Request Flow:
// Client (React / Postman)
//   → Controller
//     → Service
//       → Repository
//         → Database
// =============================================================



// =============================================================
// USING STATEMENTS
// =============================================================

// Contains DTO classes (LoginDto, SignupDto)
// DTO = Data Transfer Object (used only for request/response)
using ManuBackend.DTOs;

// Contains business logic interface (IAuthService)
using ManuBackend.Services;

// Base controller features, routing, HTTP result types
using Microsoft.AspNetCore.Mvc;

// Enables Rate Limiting attributes
using Microsoft.AspNetCore.RateLimiting;



// =============================================================
// NAMESPACE
// =============================================================

// Controllers are grouped under Controllers namespace
namespace ManuBackend.Controllers
{
    // =========================================================
    // CONTROLLER ATTRIBUTES
    // =========================================================

    // [ApiController] tells ASP.NET Core:
    //
    // 1️⃣ Automatically validate request body
    // 2️⃣ Automatically return 400 if model is invalid
    // 3️⃣ Automatically bind JSON → C# object
    // 4️⃣ Use standard API error responses
    //
    // Without this, you must manually check ModelState
    [ApiController]

    // Defines base route for this controller
    //
    // "api/[controller]" →
    // [controller] = class name without "Controller"
    //
    // AuthController → "auth"
    //
    // Final base URL:
    // /api/auth
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        // =====================================================
        // PRIVATE FIELDS
        // =====================================================

        // private  → accessible only inside this class
        // readonly → value assigned once (constructor only)
        //
        // IAuthService is an INTERFACE.
        // Actual implementation (AuthService) is injected by ASP.NET.
        private readonly IAuthService _authService;



        // =====================================================
        // CONSTRUCTOR (DEPENDENCY INJECTION)
        // =====================================================

        // When a request comes to this controller,
        // ASP.NET Core:
        // 1️⃣ Creates AuthController
        // 2️⃣ Looks at constructor parameters
        // 3️⃣ Finds registered service for IAuthService
        // 4️⃣ Injects AuthService instance automatically
        //
        // This is called Dependency Injection (DI)
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }



        // =====================================================
        // LOGIN API
        // =====================================================

        // [HttpPost("login")]
        //
        // This maps HTTP POST request to:
        // /api/auth/login
        [HttpPost("login")]

        // Limits how frequently this endpoint can be called
        // Protects against brute-force attacks
        [EnableRateLimiting("fixed")]

        // async → method runs asynchronously (non-blocking)
        //
        // Task<IActionResult> means:
        // - Method returns later (Task)
        // - Can return any HTTP response type (IActionResult)
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            // [FromBody] means:
            // ASP.NET reads JSON from request body
            // and converts it into LoginDto object
            //
            // Example request JSON:
            // {
            //   "email": "user@gmail.com",
            //   "password": "123456"
            // }

            try
            {
                // Call Service layer
                //
                // await:
                // - Pauses execution here
                // - Does NOT block server thread
                // - Continues when task completes
                var response = await _authService.LoginAsync(dto);

                // If login successful:
                // Return HTTP 200 OK with response body
                return Ok(response);
            }

            // This exception is intentionally thrown
            // when email or password is wrong
            catch (UnauthorizedAccessException ex)
            {
                // HTTP 401 Unauthorized
                return Unauthorized(new
                {
                    message = ex.Message
                });
            }

            // Catch any unexpected error
            catch (Exception ex)
            {
                // HTTP 500 Internal Server Error
                return StatusCode(500, new
                {
                    message = ex.Message
                });
            }
        }



        // =====================================================
        // SIGNUP API
        // =====================================================

        // POST /api/auth/signup
        [HttpPost("signup")]

        // Rate limit signup to avoid spam registrations
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> Signup([FromBody] SignupDto dto)
        {
            try
            {
                // Call Service layer to create new user
                var response = await _authService.SignupAsync(dto);

                // HTTP 201 Created
                //
                // Used when a new resource is created successfully
                return StatusCode(201, response);
            }

            // Thrown when:
            // - Email already exists
            // - Business rule violation
            catch (InvalidOperationException ex)
            {
                // HTTP 400 Bad Request
                return BadRequest(new
                {
                    message = ex.Message
                });
            }

            // Any unexpected server error
            catch (Exception ex)
            {
                // HTTP 500 Internal Server Error
                return StatusCode(500, new
                {
                    message = ex.Message
                });
            }
        }
    }
}