// The Controller receives HTTP requests, calls the service, and returns responses  
// It is the "entry point" for the client  

using ManuBackend.Services;
using Microsoft.AspNetCore.Mvc;
using ManuBackend.DTOs;

namespace ManuBackend.Controllers
{
    // [ApiController] adds automatic validation and error formatting  
    // [Route] sets the URL prefix: /api/auth  
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // -------------------- POST /api/auth/login --------------------  
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var response = await _authService.LoginAsync(dto);

                // 200 OK with the response data  
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                // 401 Unauthorized - wrong credentials  
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // 500 Internal Server Error - unexpected error  
                return StatusCode(500, new { message = ex.Message });
            }
        }

        // -------------------- POST /api/auth/signup --------------------  
        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupDto dto)
        {
            try
            {
                var response = await _authService.SignupAsync(dto);

                // 201 Created - new resource was created  
                return StatusCode(201, response);
            }
            catch (InvalidOperationException ex)
            {
                // 400 Bad Request - e.g. email already taken  
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}