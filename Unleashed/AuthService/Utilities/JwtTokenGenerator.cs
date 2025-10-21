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
        private readonly ILogger<JwtTokenGenerator> _logger;

        public JwtTokenGenerator(IConfiguration configuration, ILogger<JwtTokenGenerator> logger)
        {
            _configuration = configuration;
            _logger = logger;
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

        public string GenerateEmailToken(Guid userId, string userEmail)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()), 
            new Claim(JwtRegisteredClaimNames.Email, userEmail)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["EmailToken:Key"]));
            var token = new JwtSecurityToken(
                //issuer: _settings.Issuer,
                audience: "ConfirmEmail",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["EmailToken:Key"]));
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,

                    ValidateIssuer = false,
                    //ValidIssuer = _settings.Issuer,

                    ValidateAudience = true,
                    ValidAudience = "ConfirmEmail", 

                    ValidateLifetime = true, 
                    ClockSkew = TimeSpan.Zero 
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.ToString());   
                return null;
            }
        }
    }
}
