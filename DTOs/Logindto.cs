// DTO = Data Transfer Object  
// This defines the shape of data the client must send when logging in  
// We use DTOs so we never expose the full User model directly  

using System.ComponentModel.DataAnnotations;

namespace ManuBackend.DTOs
{
    public class LoginDto
    {
        // [Required] means this field cannot be empty  
        // [EmailAddress] validates that it looks like an email  
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}