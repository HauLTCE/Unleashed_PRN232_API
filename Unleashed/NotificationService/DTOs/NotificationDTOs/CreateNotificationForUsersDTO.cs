using System.Collections.Generic;
using NotificationService.DTOs.NotificationDTOs;

namespace NotificationService.DTOs
{
    public class CreateNotificationForUsersDTO : CreateNotificationDTO
    {
        public List<string> Usernames { get; set; }
    }
}