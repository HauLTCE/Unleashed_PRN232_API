using System.ComponentModel.DataAnnotations;

namespace NotificationService.DTOs.NotificationUserDTOs
{
    public class CreateNotificationUserDTO
    {
        [Required]
        public int? NotificationId { get; set; }
        [Required]
        public Guid? UserId { get; set; }

    }
}