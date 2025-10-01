using System.ComponentModel.DataAnnotations;

namespace AuthService.DTOs.RoleDTOs
{
    public class CreateRoleDTO
    {
        [Required]
        [StringLength(50)]
        public required string RoleName { get; set; }
    }
}