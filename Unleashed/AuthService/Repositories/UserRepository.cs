using AuthService.Data;
using AuthService.DTOs.UserDTOs;
using AuthService.Models;
using AuthService.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;

namespace AuthService.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AuthDbContext _authDbContext;
        public UserRepository(AuthDbContext authDbContext) {
            _authDbContext = authDbContext;
        }

        public async Task<(IEnumerable<User> entities, int totalCount)> GetPagedAsync(
        int pageNumber, 
        int pageSize, 
        string? searchQuery)
        {
            IQueryable<User> query = _authDbContext.Users.Include(u => u.Role).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var lowerCaseSearchTerm = searchQuery.Trim().ToLower();
                query = query.Where(u =>
                    (u.UserFullname != null && u.UserFullname.ToLower().Contains(lowerCaseSearchTerm)) ||
                    (u.UserUsername != null && u.UserUsername.ToLower().Contains(lowerCaseSearchTerm)) ||
                    (u.UserEmail != null && u.UserEmail.ToLower().Contains(lowerCaseSearchTerm))
                );
            }

            var totalRecords = await query.CountAsync();

            var pagedQuery = query
                .OrderBy(u => u.IsUserEnabled)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            var items = pagedQuery.AsEnumerable();

            return (items, totalRecords);
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

        public async Task<User?> GetByEmail(string email)
        {
            return await _authDbContext.Users
                 .Include(u => u.Role)
                 .FirstOrDefaultAsync(u => u.UserEmail.ToLower().Equals(email.ToLower()));
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
                return await _authDbContext.SaveChangesAsync() > 0;
            }
            catch
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

        public async Task<IEnumerable<User>> All()
        {
          return await _authDbContext.Users.Include(u => u.Role).ToListAsync();
        }

        public async Task<IEnumerable<User>> GetByIdsAsync(IEnumerable<Guid> ids)
        {
            if (ids == null || !ids.Any())
            {
                return Enumerable.Empty<User>();
            }

            // Use Where with Contains to fetch all users whose UserId is in the provided list
            return await _authDbContext.Users
                                       .Include(u => u.Role)
                                       .Where(u => ids.Contains(u.UserId))
                                       .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetByRoleId(int roleId)
        {
            // Use Where with Contains to fetch all users whose UserId is in the provided list
            return await _authDbContext.Users
                                       .Include(u => u.Role)
                                       .Where(u => u.RoleId == roleId)
                                       .ToListAsync();
        }
        public async Task<IEnumerable<UserReviewDTO>> GetUserReviewInfoByIdsAsync(IEnumerable<Guid> userIds)
        {
            if (userIds == null || !userIds.Any())
            {
                return Enumerable.Empty<UserReviewDTO>();
            }

            // Dùng .Select() để chỉ lấy các trường cần thiết, rất hiệu quả
            return await _authDbContext.Users
                .Where(u => userIds.Contains(u.UserId))
                .Select(u => new UserReviewDTO
                {
                    UserId = u.UserId,
                    UserFullname = u.UserFullname,
                    UserImage = u.UserImage
                })
                .ToListAsync();
        }
    }
}
