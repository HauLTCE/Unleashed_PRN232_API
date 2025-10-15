using System.ComponentModel.DataAnnotations;

namespace AuthService.DTOs.AuthenDTOs
{
    public class ForgotPasswordDTO
    {
        /// <summary>
        /// The email address of the user who forgot their password.
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
