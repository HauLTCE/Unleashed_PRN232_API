using System.ComponentModel.DataAnnotations;

namespace EmailService.Models.External
{
    public class ConfirmRegister
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Url] // Ensures it's a valid URL format
        public string CallbackUrl { get; set; }

    }
}
