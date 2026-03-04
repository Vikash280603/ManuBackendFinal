using System.ComponentModel.DataAnnotations;

namespace ManuBackend.DTOs
{
    public class LoginDto
    {
        // ============================================================
        // EMAIL VALIDATION
        // ============================================================
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        // ============================================================
        // PASSWORD VALIDATION
        // ============================================================
        // Note: For login, we don't enforce password complexity
        // (user might have old password before rules changed)
        // We just check it's not empty
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }
}