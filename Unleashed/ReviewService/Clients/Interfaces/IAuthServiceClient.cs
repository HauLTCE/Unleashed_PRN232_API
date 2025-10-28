using ReviewService.DTOs.External;

namespace ReviewService.Clients.Interfaces
{
    public interface IAuthServiceClient
    {
        Task<IEnumerable<UserDto>> GetUsersByIdsAsync(IEnumerable<Guid> userIds);
    }
}