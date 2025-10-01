using System.ComponentModel.DataAnnotations;

namespace NotificationService.DTOs
{
    public class CreateNotificationDTO
    {
        public Guid? UserIdSender { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "Title cannot exceed 255 characters.")]
        public string NotificationTitle { get; set; }

        [Required]
        public string NotificationContent { get; set; }

        public bool IsNotificationDraft { get; set; } = false;
    }
}