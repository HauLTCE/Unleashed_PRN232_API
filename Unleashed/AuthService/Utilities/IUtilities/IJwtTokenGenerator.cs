using AuthService.DTOs.AuthenDTOs;
using AuthService.DTOs.UserDTOs;
using AuthService.Models;
using System.Security.Claims;
namespace AuthService.Utilities.IUtilities
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
        string GenerateEmailToken(Guid userId, string userEmail);
        ClaimsPrincipal? ValidateToken(string token);
    }
}
