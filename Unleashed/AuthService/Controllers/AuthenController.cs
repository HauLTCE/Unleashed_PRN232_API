using Microsoft.AspNetCore.Mvc;
using AuthService.DTOs.AuthenDTOs;
using AuthService.DTOs.UserDTOs;
using AuthService.Services.IServices;
using System.Threading.Tasks;

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
        [ProducesResponseType(typeof(UserDTO), 200)]
        [ProducesResponseType(400)]
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
        [ProducesResponseType(typeof(LoginUserResponeDTO), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO loginDto)
        {
            var loginResponse = await _authenService.LoginAsync(loginDto);

            if (loginResponse == null)
            {
                return BadRequest("Invalid username or password.");
            }

            return Ok(loginResponse);
        }
    }
}