using NotificationService.DTOs.NotificationUserDTOs;

namespace NotificationService.Services.IServices
{
    public interface INotificationUserService
    {
        Task<IEnumerable<NotificationUserDTO>> GetAll();
        Task<NotificationUserDTO?> GetById(int notificationId, Guid userId);
        Task<NotificationUserDTO?> Create(CreateNotificationUserDTO createDto);
        Task<bool> Update(int notificationId, Guid userId, UpdateNotificationUserDTO updateDto);
        Task<bool> Delete(int notificationId, Guid userId);
    }
}