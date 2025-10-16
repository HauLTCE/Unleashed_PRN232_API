using Microsoft.AspNetCore.Mvc;
using AuthService.DTOs.AuthenDTOs;
using AuthService.DTOs.UserDTOs;
using AuthService.Services.IServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // Required for [Authorize]
using System.Security.Claims; // Required for ClaimTypes
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging; // Required for StatusCodes

namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenController : ControllerBase
    {
        private readonly IAuthenService _authenService;
        private readonly ILogger<AuthenController> _logger;

        public AuthenController(IAuthenService authenService, ILogger<AuthenController> logger)
        {
            _authenService = authenService;
            _logger = logger;
        }

        /// <summary>
        /// Registers a new user account.
        /// </summary>
        /// <param name="createUserDto">The user's registration details.</param>
        /// <returns>The newly created user's public data.</returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] CreateUserDTO createUserDto)
        {
            var createdUser = await _authenService.RegisterAsync(createUserDto);

            if (createdUser == null)
            {
                return BadRequest("Registration failed. The username or email may already be in use.");
            }

            return Ok(createdUser);
        }

        /// <summary>
        /// Authenticates a user and returns a JWT.
        /// </summary>
        /// <param name="loginDto">The user's login credentials.</param>
        /// <returns>A response containing the JWT and basic user info.</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginUserResponeDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO loginDto)
        {
            var loginResponse = await _authenService.LoginAsync(loginDto);

            if (loginResponse == null)
            {
                return BadRequest("Invalid username or password.");
            }

            return Ok(loginResponse);
        }

        /// <summary>
        /// Checks if the current user's JWT is valid. Requires a valid JWT in the Authorization header.
        /// </summary>
        /// <returns>A success message with the authenticated user's ID.</returns>
        [HttpGet("check-auth")]
        [Authorize] // This attribute now protects the endpoint
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult CheckAuthorization()
        {
            var roleName = User.FindFirst(ClaimTypes.Role)?.Value;
            return Ok(roleName);
        }

        [HttpPost("google-login")]
        [ProducesResponseType(typeof(LoginUserResponeDTO), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> LoginWithGoogle([FromBody] GoogleLoginDTO googleLoginDto)
        {
            try
            {
                var loginResponse = await _authenService.LoginWithGoogleAsync(googleLoginDto);

                if (loginResponse == null)
                {
                    // This indicates authentication failed (e.g., invalid Google token)
                    return Unauthorized(new { message = "Google authentication failed." });
                }

                // Authentication was successful, return the JWT
                return Ok(loginResponse);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) with your logging framework
                return StatusCode(500, new { message = "An internal server error occurred." });
            }
        }

        /* // NOTE: The corresponding service method 'ResetPasswordAsync' is commented out in AuthenService.
       // Uncomment this endpoint once the service-level implementation is complete.

       /// <summary>
       /// Resets a user's password using a valid reset token.
       /// </summary>
       /// <param name="resetPasswordDto">The reset token and the new password.</param>
       /// <returns>A success or failure message.</returns>
       [HttpPost("reset-password")]
       [ProducesResponseType(StatusCodes.Status200OK)]
       [ProducesResponseType(StatusCodes.Status400BadRequest)]
       public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDto)
       {
           var result = await _authenService.ResetPasswordAsync(resetPasswordDto);

           if (!result)
           {
               return BadRequest(new { message = "Invalid or expired password reset token." });
           }

           return Ok(new { message = "Password has been reset successfully." });
       }
       */

        /// <summary>
        /// Changes the password for the currently authenticated user.
        /// </summary>
        /// <param name="changePasswordDto">An object containing the user's old and new passwords.</param>
        /// <returns>A success or failure message.</returns>
        [HttpPost("change-password")]
        [Authorize] 
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDto)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                // This case should be rare if [Authorize] is working, but it's a good safeguard.
                return Unauthorized(new { message = "User identifier claim is missing or invalid." });
            }

            var result = await _authenService.ChangePasswordAsync(userId, changePasswordDto);

            if (!result)
            {
                return BadRequest(new { message = "Failed to change password. Please verify your current password." });
            }

            return Ok(new { message = "Password changed successfully." });
        }
    }
}

