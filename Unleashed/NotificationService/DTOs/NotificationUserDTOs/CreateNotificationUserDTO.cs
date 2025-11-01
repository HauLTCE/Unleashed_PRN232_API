using System.ComponentModel.DataAnnotations;

namespace NotificationService.DTOs.NotificationUserDTOs
{
    public class CreateNotificationUserDTO
    {
        [Required(ErrorMessage = "The notification is required.")]
        public int? NotificationId { get; set; }

    }
}