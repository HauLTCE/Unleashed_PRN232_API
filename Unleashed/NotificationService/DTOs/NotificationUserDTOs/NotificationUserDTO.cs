using NotificationService.DTOs.NotificationDTOs;

namespace NotificationService.DTOs.NotificationUserDTOs
{
    public class NotificationUserDTO
    {
        public int? NotificationId { get; set; }
        public Guid? UserId { get; set; }
        public bool? IsNotificationViewed { get; set; }
        public bool? IsNotificationDeleted { get; set; }
        public NotificationDTO? NotificationDTO { get; set; }
    }
}
