using System.ComponentModel.DataAnnotations;

namespace AuthService.DTOs.UserDTOs;

public class CreateExternalUserDTO
{
    [Required]
    [EmailAddress]
    public required string UserEmail { get; set; }

    [Required]
    [StringLength(150)]
    public required string UserFullname { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 3)]
    public required string UserUsername { get; set; }
}