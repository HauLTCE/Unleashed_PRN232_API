using Microsoft.AspNetCore.Mvc;
using AuthService.DTOs.AuthenDTOs;
using AuthService.DTOs.UserDTOs;
using AuthService.Services.IServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // Required for [Authorize]
using System.Security.Claims; // Required for ClaimTypes
using Microsoft.AspNetCore.Http; // Required for StatusCodes

namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenController : ControllerBase
    {
        private readonly IAuthenService _authenService;

        public AuthenController(IAuthenService authenService)
        {
            _authenService = authenService;
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
        [Authorize] // This attribute protects the endpoint
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult CheckAuthorization()
        {
            // The [Authorize] attribute handles JWT validation. 
            // If the code reaches this point, the user is authenticated.

            // You can access user claims from the HttpContext.User principal
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var roleName = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userId == null)
            {
                // This is a safeguard, but the authorize attribute should prevent this.
                return Unauthorized();
            }

            return Ok($"Authorization successful. Welcome, {username} (ID: {userId}) with role {roleName}!");
        }
    }
}