using System.ComponentModel.DataAnnotations;

namespace AuthService.DTOs.UserDTOs
{
    public class ChangPasswordUserDTO
    {
            [Required]
            public string? OldPassword { get; set; }

            [Required]
            [StringLength(100, MinimumLength = 8, ErrorMessage = "New password must be at least 8 characters long.")]
            public string? NewPassword { get; set; }

            [Required]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string? ConfirmNewPassword { get; set; }
    }
}
