using AuthService.Models;
using AuthService.Utilities.IUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Utilities
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IConfiguration _configuration;

        public JwtTokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            // 1. Define Claims
            // These are the "facts" about the user that will be stored in the token.
            var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()), // Subject (standard claim for user ID)
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT ID (ensures token is unique)
            new(ClaimTypes.Name,  user.UserFullname),
            new(ClaimTypes.Role, user.Role.RoleName)
            //Can add more claims here, like roles
            // new Claim(ClaimTypes.Role, user.Role.RoleName)
        };

            // 2. Get the secret key from appsettings.json
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            // 3. Create signing credentials
            // The key is combined with a strong algorithm to create a digital signature.
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            // 4. Define Token Expiration
            var expires = DateTime.UtcNow.AddHours(1); // Token is valid for 1 hour

            // 5. Create the token descriptor
            // This brings together all the token's properties.
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expires,
                SigningCredentials = creds,
            };

            // 6. Generate the token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // 7. Write the token to a string format
            return tokenHandler.WriteToken(token);
        }
    }
}
