using ReviewService.DTOs.External;

namespace ReviewService.Clients.Interfaces
{
    public interface INotificationServiceClient
    {
        Task<bool> CreateNotificationForUsersAsync(CreateNotificationForUsersRequestDto createDto);
    }
}