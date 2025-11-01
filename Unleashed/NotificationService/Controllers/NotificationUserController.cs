using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.DTOs.NotificationUserDTOs;
using NotificationService.DTOs.PagedResponse;
using NotificationService.Services.IServices;
using System.Security.Claims;

namespace NotificationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationUsersController : ControllerBase
    {
        private readonly INotificationUserService _service;

        public NotificationUsersController(INotificationUserService service)
        {
            _service = service;
        }

        // GET: api/NotificationUsers
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<NotificationUserDTO>), StatusCodes.Status200OK)]
        [Authorize(Roles = "ADMIN, STAFF")]
        public async Task<ActionResult<IEnumerable<NotificationUserDTO>>> GetNotificationUsers()
        {
            var result = await _service.GetAll();
            return Ok(result);
        }

        // GET: api/NotificationUsers/userId
        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(NotificationUserPagedResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<NotificationUserPagedResponse>> GetNotificationUsersByUserId(
        Guid userId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchQuery = null)
        {
            var result = await _service.GetNotificationUserByUserIdPagedAsync(userId, pageNumber, pageSize, searchQuery);
            return Ok(result);
        }


        // GET: api/NotificationUsers/5/guid
        [HttpGet("{notificationId}/{userId}")]
        [ProducesResponseType(typeof(NotificationUserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<NotificationUserDTO>> GetNotificationUser(int notificationId, Guid userId)
        {
            var result = await _service.GetById(notificationId, userId);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        // POST: api/NotificationUsers
        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<NotificationUserDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "ADMIN, STAFF")]
        public async Task<IActionResult> PostNotificationUser([FromQuery] int notificationId)
        {
            var newRecord = await _service.Create(notificationId);

            if (newRecord == null || !newRecord.Any())
            {
                return BadRequest("Failed to create the notification user record.");
            }

            return CreatedAtAction(
                nameof(GetNotificationUsers),  
                new { notificationId },
                newRecord                      
            );
        }


        // PUT: api/NotificationUsers/5/guid
        [HttpPut("{notificationId}/{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutNotificationUser(int notificationId, Guid userId, UpdateNotificationUserDTO updateDto)
        {
            var claimsUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (claimsUserId != userId.ToString())
            {
                return Forbid();
            }
            var success = await _service.Update(notificationId, userId, updateDto);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }

        // DELETE: api/NotificationUsers/5/guid
        [HttpDelete("{notificationId}/{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteNotificationUser(int notificationId, Guid userId)
        {
            var claimsUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (claimsUserId != userId.ToString())
            {
                return Forbid();
            }
            var success = await _service.Delete(notificationId, userId);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
