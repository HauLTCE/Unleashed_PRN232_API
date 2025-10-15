using AuthService.Models;

namespace AuthService.Repositories.IRepositories
{
    public interface IUserRepository : IGenericRepository<Guid, User>
    {
        Task<User?> GetByUsername(string username);
        Task<User?> GetByEmail(string email);
    }
}
