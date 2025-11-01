using NotificationService.DTOs.NotificationUserDTOs;

namespace NotificationService.DTOs.PagedResponse
{
    public class NotificationUserPagedResponse(
        List<NotificationUserDTO> items,
        int totalCount = 0,
        int unviewCount = 0,
        int pageNumber = 0,
        int pageSize = 10
        ) : PagedResponse<NotificationUserDTO>(items, totalCount, pageNumber, pageSize)
    {
        public int UnviewCount { get; set; } = unviewCount;
    }
}
