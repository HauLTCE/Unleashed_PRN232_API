using AuthService.DTOs.AuthenDTOs;
using AuthService.DTOs.UserDTOs;
using AuthService.Models;
using AuthService.Services.IServices;
using AuthService.Utilities.IUtilities;
using Microsoft.AspNetCore.Authorization; // Required for [Authorize]
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims; // Required for ClaimTypes
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenController : ControllerBase
    {
        private readonly IAuthenService _authenService;
        private readonly IUserService _userService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly ILogger<AuthenController> _logger;

        public AuthenController(IAuthenService authenService, 
            IUserService userService,
            IHttpClientFactory httpClientFactory,
            IJwtTokenGenerator jwtTokenGenerator,
            ILogger<AuthenController> logger)
        {
            _authenService = authenService;
            _userService = userService;
            _jwtTokenGenerator = jwtTokenGenerator;
            _httpClientFactory = httpClientFactory;
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

            try
            {
                var token = _jwtTokenGenerator.GenerateEmailToken(createdUser.UserId, createdUser.UserEmail);

                var callbackUrl = Url.Action(
                    action: nameof(ConfirmEmail),
                    controller: "Authen",
                    values: new { userId = createdUser.UserId, token = token },
                    protocol: Request.Scheme);

                if (callbackUrl == null)
                {
                    _logger.LogError("Could not create callback URL for user {Email}", createdUser.UserEmail);
                }
                else
                {
                    var emailRequest = new 
                    {
                        Email = createdUser.UserEmail,
                        CallbackUrl = callbackUrl
                    };
                    // 1. Create the HttpClient
                    var httpClient = _httpClientFactory.CreateClient("EmailService");

                    // 2. Call the Email Service API
                    //    Assuming the endpoint is '/api/email/send'
                    var response = await httpClient.PostAsJsonAsync("/api/SendEmail/send-confirm-register", emailRequest);

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogError("Failed to send confirmation email. EmailService responded with {StatusCode}", response.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling EmailService for user {Email}", createUserDto.UserEmail);
                // Don't fail the registration, just log the error.
            }

            return Ok(createdUser);
        }

        [HttpGet("confirm-email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // We only need the token from the query string
        public async Task<IActionResult> ConfirmEmail([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { message = "Invalid confirmation link." });
            }

            // 1. Validate the JWT
            var principal = _jwtTokenGenerator.ValidateToken(token);

            if (principal == null)
            {
                _logger.LogWarning("Invalid email confirmation token received.");
                return BadRequest(new { message = "Invalid or expired confirmation link." });
            }

            // 2. Get the UserId from the token's claims
            var userIdString = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdString == null || !Guid.TryParse(userIdString, out Guid userId))
            {
                _logger.LogError("Email token is valid but does not contain a valid UserId.");
                return BadRequest(new { message = "Invalid token data." });
            }

            // 3. SECURELY validate and enable the user
            // We pass the UserId we extracted from the token.
            var result = await _authenService.ConfirmEmailAsync(userId);

            if (result)
            {
                _logger.LogInformation("Email confirmed!");
                return Ok(new { message = "Email confirmed successfully!" });
            }

            _logger.LogWarning("Email confirmation failed!");
            return BadRequest(new { message = "Email confirmation failed." });
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

