using System.ComponentModel.DataAnnotations;

namespace AuthService.DTOs.UserDTOs
{
    public class UpdateUserDTO
    {
        [StringLength(255)]
        public string? UserFullname { get; set; }

        [Phone]
        [StringLength(12)]
        public string? UserPhone { get; set; }

        [StringLength(255)]
        public string? UserBirthdate { get; set; }

        [StringLength(255)]
        public string? UserAddress { get; set; }

        [Url]
        [StringLength(255)]
        public string? UserImage { get; set; }
        [Required]
        public required int RoleId { get; set; }
    }
}
