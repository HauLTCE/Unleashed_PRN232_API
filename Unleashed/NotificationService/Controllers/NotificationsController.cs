using Microsoft.AspNetCore.Mvc;
using NotificationService.DTOs.NotificationDTOs;
using NotificationService.Services.IServices;

namespace NotificationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // GET: api/Notifications
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<NotificationDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<NotificationDTO>>> GetNotifications()
        {
            var notifications = await _notificationService.GetAllNotifications();
            return Ok(notifications);
        }

        // GET: api/Notifications/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(NotificationDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<NotificationDTO>> GetNotification(int id)
        {
            var notification = await _notificationService.GetNotificationById(id);

            if (notification == null)
            {
                return NotFound();
            }

            return Ok(notification);
        }

        // POST: api/Notifications
        [HttpPost]
        [ProducesResponseType(typeof(NotificationDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<NotificationDTO>> PostNotification(CreateNotificationDTO createDto)
        {
            var newNotification = await _notificationService.CreateNotification(createDto);

            if (newNotification == null)
            {
                return BadRequest("Failed to create the notification.");
            }

            return CreatedAtAction(nameof(GetNotification), new { id = newNotification.NotificationId }, newNotification);
        }

        // PUT: api/Notifications/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutNotification(int id, UpdateNotificationDTO updateDto)
        {
            var success = await _notificationService.UpdateNotification(id, updateDto);

            if (!success)
            {
                return NotFound("Notification not found or update failed.");
            }

            return NoContent();
        }

        // DELETE: api/Notifications/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var success = await _notificationService.DeleteNotification(id);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
