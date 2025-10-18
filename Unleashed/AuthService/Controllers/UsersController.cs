using AuthService.DTOs.PagedResponse;
using AuthService.DTOs.UserDTOs;
using AuthService.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http; // Added for StatusCodes
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase //lacking a self modification. self mods check for if the user is the same one as the one being mod. if yes, allow, if no, fuck no
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/Users
        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<UserDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResponse<UserDTO>>> GetUsers(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchQuery = null)
        {
            var query = _userService.GetAll();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var lowerCaseSearchTerm = searchQuery.Trim().ToLower();
                query = query.Where(u =>
                    (u.UserFullname != null && u.UserFullname.ToLower().Contains(lowerCaseSearchTerm)) ||
                    (u.UserUsername != null && u.UserUsername.ToLower().Contains(lowerCaseSearchTerm)) ||
                    (u.UserEmail != null && u.UserEmail.ToLower().Contains(lowerCaseSearchTerm))
                );
            }

            var totalRecords = await query.CountAsync();
            
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var pagedResponse = new PagedResponse<UserDTO>(items, totalRecords, pageNumber, pageSize);

            return Ok(pagedResponse);
        }

        // GET: api/Users/5
        [Authorize] 
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDTO>> GetUser(Guid id)
        {
           
            var claimsUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!User.IsInRole("ADMIN") && claimsUserId != id.ToString())
            {
                // If they are not an Admin AND not requesting their own info, deny access.
                return Forbid();
            }

            var user = await _userService.GetById(id);

            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            return Ok(user);
        }

        // GET: api/Users/ByUsername/me
        [HttpGet("ByUsername/{username}")] //what the fuck bro?
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
        [Authorize(Roles = "ADMIN")]
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
        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutUser(Guid id, UpdateUserDTO updateUserDto)
        {
            var claimsUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (claimsUserId != id.ToString())
            {
                // If they are not requesting their own info, deny access.
                return Forbid();
            }
            var success = await _userService.UpdateUser(id, updateUserDto);

            if (!success)
            {
                return NotFound($"User with ID {id} not found.");
            }

            return NoContent();
        }

        // DELETE: api/Users/5
        [Authorize(Roles = "ADMIN, CUSTOMER")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var claimsUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!User.IsInRole("ADMIN") && claimsUserId != id.ToString())
            {
                // If they are not ADMIN OR not requesting their own info, deny access.
                return Forbid();
            }

            var success = await _userService.DeleteUser(id);

            if (!success)
            {
                return NotFound($"User with ID {id} not found.");
            }

            return NoContent();
        }
    }
}