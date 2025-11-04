using System.Collections.Generic;

namespace ReviewService.DTOs.External
{
    public class CreateNotificationForUsersRequestDto : CreateNotificationDTO
    {
        public List<string> Usernames { get; set; } = new();
    }
}