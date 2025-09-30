using AuthService.Data;
using AuthService.Models;
using AuthService.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AuthDbContext _authDbContext;

        public RoleRepository(AuthDbContext authDbContext)
        {
            _authDbContext = authDbContext;
        }
        public IQueryable<Role> All()
        {
            return _authDbContext.Roles.AsQueryable(); 
        }

        public async Task<bool> CreateAsync(Role entity)
        {
            try
            {
                await _authDbContext.Roles.AddAsync(entity);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Delete(Role entity)
        {
            try
            {
                _authDbContext.Roles.Remove(entity);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Role?> FindAsync(int id)
        {
            return await _authDbContext.Roles.FirstOrDefaultAsync(x => x.RoleId == id);
        }

        public async Task<bool> IsAny(int id)
        {
            return await _authDbContext.Roles.AnyAsync(x => x.RoleId == id);
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

        public bool Update(Role entity)
        {
            try
            {
                _authDbContext.Roles.Update(entity);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
