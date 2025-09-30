using System.ComponentModel.DataAnnotations;

namespace AuthService.DTOs.AuthenDTOs
{
    public class LoginUserDTO
    {
        [Required]
        public required string Username { get; set; }

        [Required]
        public required string Password { get; set; }
    }
}
