using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AuthService.Services.IServices;
using AuthService.DTOs.UserDTOs;
using Microsoft.AspNetCore.Http; // Added for StatusCodes

namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/Users
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var users = await _userService.GetAll();
            return Ok(users);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDTO>> GetUser(Guid id)
        {
            var user = await _userService.GetById(id);

            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            return Ok(user);
        }

        // GET: api/Users/ByUsername/me
        [HttpGet("ByUsername/{username}")]
        [ProducesResponseType(typeof(ImportServiceUserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ImportServiceUserDTO>> GetByUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Username cannot be empty.");
            }

            var user = await _userService.GetByUsernameForImportService(username);

            if (user == null)
            {
                return NotFound($"User with username '{username}' not found.");
            }

            return Ok(user);
        }

        // POST: api/Users
        [HttpPost]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserDTO>> PostUser(CreateUserDTO createUserDto)
        {
            var createdUser = await _userService.CreateUser(createUserDto);

            if (createdUser == null)
            {
                return BadRequest("Could not create the user. Please check your input.");
            }

            return CreatedAtAction(nameof(GetUser), new { id = createdUser.UserId }, createdUser);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutUser(Guid id, UpdateUserDTO updateUserDto)
        {
            var success = await _userService.UpdateUser(id, updateUserDto);

            if (!success)
            {
                return NotFound($"User with ID {id} not found.");
            }

            return NoContent();
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var success = await _userService.DeleteUser(id);

            if (!success)
            {
                return NotFound($"User with ID {id} not found.");
            }

            return NoContent();
        }
    }
}