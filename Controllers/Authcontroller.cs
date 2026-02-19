// -------------------------------------------------------------
// CONTROLLER = ENTRY POINT OF HTTP REQUESTS
// -------------------------------------------------------------
// Controller receives HTTP requests from client (React/Postman),
// calls Service layer,
// and returns HTTP response.
//
// Flow:
// Client → Controller → Service → Repository → Database
// -------------------------------------------------------------
     


// Gives access to IAuthService (Service layer)
using ManuBackend.Services;

// Gives access to ControllerBase, IActionResult, Http attributes
using Microsoft.AspNetCore.Mvc;

// Gives access to DTO classes (LoginDto, SignupDto)
using ManuBackend.DTOs;



namespace ManuBackend.Controllers
{
    // -------------------------------------------------------------
    // [ApiController]
    // Enables:
    // 1. Automatic Model Validation
    // 2. Automatic 400 Bad Request for invalid model
    // 3. Automatic binding from body
    // 4. Standard error formatting
    // -------------------------------------------------------------
    [ApiController]

    // -------------------------------------------------------------
    // [Route("api/[controller]")]
    // Defines base URL for this controller.
    //
    // [controller] automatically becomes class name without "Controller"
    //
    // So AuthController → "auth"
    //
    // Final Base URL:
    // /api/auth
    // -------------------------------------------------------------
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        // -------------------------------------------------------------
        // private = accessible only inside this class
        // readonly = value cannot be changed after constructor
        //
        // IAuthService is interface (abstraction)
        // Actual object injected by Dependency Injection
        // -------------------------------------------------------------
        private readonly IAuthService _authService;


        // -------------------------------------------------------------
        // Constructor
        //
        // When this controller is created,
        // ASP.NET automatically injects IAuthService
        // because we registered it in Program.cs:
        //
        // builder.Services.AddScoped<IAuthService, AuthService>();
        //
        // This is Dependency Injection.
        // -------------------------------------------------------------
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }



        // =============================================================
        // -------------------- LOGIN ENDPOINT --------------------------
        // =============================================================

        // [HttpPost("login")]
        // This means:
        // POST request to:
        // /api/auth/login
        [HttpPost("login")]

        // Task<IActionResult>
        //
        // Task → because method is asynchronous (async)
        // IActionResult → allows returning different HTTP responses
        //
        // async keyword → allows use of await
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            // [FromBody] means:
            // Read JSON from request body
            //
            // Example JSON:
            // {
            //   "email": "test@gmail.com",
            //   "password": "123456"
            // }

            try
            {
                // Call Service layer method
                // await → wait until async operation completes
                var response = await _authService.LoginAsync(dto);

                // If success:
                // Return 200 OK
                return Ok(response);
            }

            // If service throws UnauthorizedAccessException
            // That means wrong email or password
            catch (UnauthorizedAccessException ex)
            {
                // Return 401 Unauthorized
                return Unauthorized(new { message = ex.Message });
            }

            // Any unexpected error
            catch (Exception ex)
            {
                // Return 500 Internal Server Error
                return StatusCode(500, new { message = ex.Message });
            }
        }



        // =============================================================
        // -------------------- SIGNUP ENDPOINT -------------------------
        // =============================================================

        // POST /api/auth/signup
        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupDto dto)
        {
            try
            {
                // Call Service layer to create user
                var response = await _authService.SignupAsync(dto);

                // 201 Created
                // Used when new resource is created
                return StatusCode(201, response);
            }

            // If email already exists
            catch (InvalidOperationException ex)
            {
                // 400 Bad Request
                return BadRequest(new { message = ex.Message });
            }

            catch (Exception ex)
            {
                // Unexpected error
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
