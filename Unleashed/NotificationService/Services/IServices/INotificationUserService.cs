using Microsoft.Extensions.Configuration.UserSecrets;
using NotificationService.DTOs.NotificationUserDTOs;
using NotificationService.DTOs.PagedResponse;

namespace NotificationService.Services.IServices
{
    public interface INotificationUserService
    {
        Task<IEnumerable<NotificationUserDTO>> GetAll();
        Task<NotificationUserDTO?> GetById(int notificationId, Guid userId);
        Task<IEnumerable<NotificationUserDTO>?> Create(int notificaitonId);
        Task<bool> Update(int notificationId, Guid userId, UpdateNotificationUserDTO updateDto);
        Task<bool> Delete(int notificationId, Guid userId);
        Task<NotificationUserPagedResponse> GetNotificationUserByUserIdPagedAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        string? searchQuery);
    }
}