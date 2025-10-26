using NotificationService.DTOs.NotificationDTOs;
using NotificationService.DTOs.PagedResponse;

namespace NotificationService.Services.IServices
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationDTO>> GetAllNotifications();
        Task<NotificationDTO?> GetNotificationById(int id);
        Task<NotificationDTO?> CreateNotification(CreateNotificationDTO createDto);
        Task<bool> UpdateNotification(int id, UpdateNotificationDTO updateDto);
        Task<bool> DeleteNotification(int id);
        Task<PagedResponse<NotificationDTO>> GetNotificationPagedAsync(
        int pageNumber,
        int pageSize,
        string? searchQuery,
        bool? isDraft);
    }
}