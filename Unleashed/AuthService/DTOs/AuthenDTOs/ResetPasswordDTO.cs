using System.ComponentModel.DataAnnotations;

namespace AuthService.DTOs.AuthenDTOs
{
    public class ResetPasswordDTO
    {
        [Required]
        public required Guid UserId { get; set; }

        [Required]
        public required string Password { get; set; }
    }
}
