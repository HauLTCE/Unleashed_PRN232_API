using AuthService.DTOs.UserDTOs;
using AuthService.Models;

namespace AuthService.Repositories.IRepositories
{
    public interface IUserRepository : IGenericRepository<Guid, User>
    {
        Task<User?> GetByUsername(string username);
        Task<User?> GetByEmail(string email);

        Task<IEnumerable<User>> GetByIdsAsync(IEnumerable<Guid> ids);
        Task<IEnumerable<User>> GetByRoleId(int roleId);

        Task<IEnumerable<UserReviewDTO>> GetUserReviewInfoByIdsAsync(IEnumerable<Guid> userIds);
    }
}
