using System.ComponentModel.DataAnnotations;

namespace AuthService.DTOs.AuthenDTOs
{
    public class GoogleLoginDTO
    {
        /// <summary>
        /// The ID token provided by the Google Sign-In client.
        /// </summary>
        [Required]
        public string IdToken { get; set; } = string.Empty;
    }
}
