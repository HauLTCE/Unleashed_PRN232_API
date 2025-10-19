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
        public async Task<IEnumerable<Role>> All()
        {
            return await _authDbContext.Roles.ToListAsync(); 
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

        public async Task<(IEnumerable<Role> entities, int totalCount)> GetPagedAsync(int pageNumber, int pageSize, string? searchQuery)
        {
            IQueryable<Role> query = _authDbContext.Roles.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var lowerCaseSearchTerm = searchQuery.Trim().ToLower();
                query = query.Where(r =>
                    (r.RoleName != null && r.RoleName.ToLower().Contains(lowerCaseSearchTerm)) 
                );
            }

            var totalRecords = await query.CountAsync();

            var pagedQuery = query
                .OrderBy(r => r.RoleId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            var items = pagedQuery.AsEnumerable();

            return (items, totalRecords);
        }

        public async Task<bool> IsAny(int id)
        {
            return await _authDbContext.Roles.AnyAsync(x => x.RoleId == id);
        }

        public async Task<bool> SaveAsync()
        {
            try
            {
                return await _authDbContext.SaveChangesAsync() > 0;
            }
            catch
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
