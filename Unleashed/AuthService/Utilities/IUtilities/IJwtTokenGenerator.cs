using AuthService.DTOs.AuthenDTOs;
using AuthService.DTOs.UserDTOs;
using AuthService.Models;
namespace AuthService.Utilities.IUtilities
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }
}
