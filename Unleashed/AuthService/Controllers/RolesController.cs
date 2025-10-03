using AuthService.DTOs.RoleDTOs;
using AuthService.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        // GET: api/Roles
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RoleDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RoleDTO>>> GetRoles()
        {
            var roles = await _roleService.GetAll();
            return Ok(roles);
        }

        // GET: api/Roles/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RoleDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RoleDTO>> GetRole(int id)
        {
            var role = await _roleService.GetById(id);

            if (role == null)
            {
                return NotFound();
            }

            return Ok(role);
        }

        // POST: api/Roles
        [HttpPost]
        [ProducesResponseType(typeof(RoleDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RoleDTO>> PostRole(CreateRoleDTO createRoleDto)
        {
            var newRole = await _roleService.CreateRole(createRoleDto);

            if (newRole == null)
            {
                return BadRequest("Failed to create the role.");
            }

            // Return a 201 Created response with a link to the new resource
            return CreatedAtAction(nameof(GetRole), new { id = newRole.RoleId }, newRole);
        }

        // PUT: api/Roles/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutRole(int id, UpdateRoleDTO updateRoleDto)
        {
            var success = await _roleService.UpdateRole(id, updateRoleDto);

            if (!success)
            {
                return NotFound("Role not found or update failed.");
            }

            return NoContent();
        }

        // DELETE: api/Roles/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var success = await _roleService.DeleteRole(id);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}