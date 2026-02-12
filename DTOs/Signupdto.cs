// Shape of data the client sends when signing up  

using System.ComponentModel.DataAnnotations;

namespace ManuBackend.DTOs
{
    public class SignupDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        // MinLength ensures password is not too short  
        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = string.Empty;

        // Role must match one of the allowed roles  
        [Required]
        public string Role { get; set; } = string.Empty;
    }
}