using AuthService.Data;
using AuthService.Models;
using AuthService.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AuthDbContext _authDbContext;
        public UserRepository(AuthDbContext authDbContext) {
        _authDbContext = authDbContext;
        }
        public IQueryable<User> All()
        {
            return _authDbContext.Users.Include(u => u.Role).AsQueryable();
        }

        public async Task<bool> CreateAsync(User entity)
        {
            try
            {
                await _authDbContext.Users.AddAsync(entity );
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
        }

        public bool Delete(User entity)
        {
            try
            {
                _authDbContext.Users.Remove(entity);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<User?> FindAsync(Guid id)
        {
            return await _authDbContext.Users
                          .Include(u => u.Role)
                          .FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<User?> GetByUsername(string username)
        {
            return await _authDbContext.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserUsername.ToLower().Equals(username.ToLower()));
        }

        public async Task<bool> IsAny(Guid id)
        {
           return await _authDbContext.Users.AnyAsync(u => u.UserId == id);
        }

        public async Task<bool> SaveAsync()
        {
            try
            {
                await _authDbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Update(User entity)
        {
            try
            {
                _authDbContext.Users.Update(entity);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
