using System.ComponentModel.DataAnnotations;

namespace NotificationService.DTOs.NotificationUserDTOs
{
    public class CreateNotificationUserDTO
    {
        [Required(ErrorMessage = "The notification is required.")]
        public int? NotificationId { get; set; }

        [Required(ErrorMessage = "The User is required.")]
        [MinLength(1, ErrorMessage = "At least one UserId must be provided.")]
        public List<Guid> UserIds { get; set; } = []; 

    }
}