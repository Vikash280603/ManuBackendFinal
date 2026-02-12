// This is what we send BACK to the client after a successful login or signup  
// We include the JWT token and some basic user info  

namespace ManuBackend.DTOs
{
    public class AuthResponseDto
    {
        // The JWT token - client stores this and sends it with every future request  
        public string Token { get; set; } = string.Empty;

        // Basic user info to display in the UI  
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}