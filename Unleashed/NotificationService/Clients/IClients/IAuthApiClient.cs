using NotificationService.DTOs.External;

namespace NotificationService.Clients.IClients
{
    public interface IAuthApiClient
    {
        Task<IEnumerable<Guid>> GetCustomers();
        Task<UserDto?> GetUserByUsernameAsync(string username);
        Task<IEnumerable<UserDto>> GetUsersByIdsAsync(IEnumerable<Guid> userIds);
    }
}
