using InventoryService.DTOs.External;

namespace InventoryService.Clients.Interfaces
{
    public interface IAuthServiceClient
    {
        Task<UserDto?> GetUserByUsernameAsync(string username);
    }
}
