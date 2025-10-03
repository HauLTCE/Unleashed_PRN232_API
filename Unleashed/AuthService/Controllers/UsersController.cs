using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AuthService.Services.IServices;
using AuthService.DTOs.UserDTOs; // Ensure you have the correct using for your DTOs

namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        // Inject the service instead of the DbContext
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var users = await _userService.GetAll();
            return Ok(users);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(Guid id)
        {
            var user = await _userService.GetById(id);

            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            return Ok(user);
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<UserDTO>> PostUser(CreateUserDTO createUserDto)
        {
            // Note: For this to work best, your IUserService.CreateUser method
            // should be modified to return the created UserDTO instead of just a boolean.
            var createdUser = await _userService.CreateUser(createUserDto);

            if (createdUser == null)
            {
                return BadRequest("Could not create the user. Please check your input.");
            }

            // Return a 201 Created status with a link to the new resource
            return CreatedAtAction(nameof(GetUser), new { id = createdUser.UserId }, createdUser);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(Guid id, UpdateUserDTO updateUserDto)
        {
            var success = await _userService.UpdateUser(id, updateUserDto);

            if (!success)
            {
                return NotFound($"User with ID {id} not found.");
            }

            return NoContent(); // Standard response for a successful PUT
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var success = await _userService.DeleteUser(id);

            if (!success)
            {
                return NotFound($"User with ID {id} not found.");
            }

            return NoContent(); // Standard response for a successful DELETE
        }
    }
}