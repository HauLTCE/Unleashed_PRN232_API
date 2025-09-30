using System.ComponentModel.DataAnnotations;

namespace AuthService.DTOs.UserDTOs;

/// <summary>
/// Defines the data required to register a new user.
/// </summary>
public class CreateUserDTO
{
    [Required]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 100 characters.")]
    public required string? UserUsername { get; set; }

    [Required]
    [EmailAddress]
    public required string? UserEmail { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
    public required string UserPassword { get; set; }

    [Required]
    [Compare("UserPassword", ErrorMessage = "The password and confirmation password do not match.")]
    public required string ConfirmPassword { get; set; }

    [Required]
    [StringLength(150)]
    public string? UserFullname { get; set; }
}