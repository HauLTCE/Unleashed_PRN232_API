using Microsoft.EntityFrameworkCore;
using NotificationService.Data;
using NotificationService.Models;
using NotificationService.Repositories.IRepositories;

namespace NotificationService.Repositories
{
    public class NotificationUserRepository : INotificationUserRepository
    {
        private readonly NotificationDbContext _context;

        public NotificationUserRepository(NotificationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<NotificationUser>> All()
        {
            return await _context.NotificationUsers.ToListAsync();
        }

        public async Task<bool> CreateAsync(NotificationUser entity)
        {
            try
            {
                await _context.NotificationUsers.AddAsync(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(NotificationUser entity)
        {
            try
            {
                _context.NotificationUsers.Remove(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<NotificationUser?> FindAsync((int, Guid) id)
        {
            var (notificationId, userId) = id;
            return await _context.NotificationUsers
                .FirstOrDefaultAsync(nu => nu.NotificationId == notificationId && nu.UserId == userId);
        }

        public async Task<(IEnumerable<NotificationUser> entities, int totalCount)> GetPagedAsync(int pageNumber, int pageSize, string? searchQuery)
        {
            IQueryable<NotificationUser> query = _context.NotificationUsers
                .Include(nu => nu.NotificationId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var lowerCaseSearchTerm = searchQuery.Trim().ToLower();
                query = query.Where(n =>
                    n.Notification.NotificationTitle != null && n.Notification.NotificationTitle.ToLower().Contains(lowerCaseSearchTerm));
            }

            var totalRecords = await query.CountAsync();

            var pagedQuery = query
                .Where(u => u.IsNotificationDeleted.GetValueOrDefault(false))
                .OrderByDescending(u => u.IsNotificationViewed)
                .ThenByDescending(u => u.Notification.NotificationCreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            var items = pagedQuery.AsEnumerable();

            return (items, totalRecords);
        }

        public async Task<(IEnumerable<NotificationUser>, int)> GetPagedByUserIdAsync(Guid userId, int pageNumber, int pageSize, string? searchQuery)
        {
            IQueryable<NotificationUser> query = _context.NotificationUsers
                 .Include(nu => nu.NotificationId)
                 .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var lowerCaseSearchTerm = searchQuery.Trim().ToLower();
                query = query.Where(n =>
                    n.Notification.NotificationTitle != null && n.Notification.NotificationTitle.ToLower().Contains(lowerCaseSearchTerm));
            }

            var totalRecords = await query.CountAsync();

            var pagedQuery = query
                .Where(u => u.IsNotificationDeleted.GetValueOrDefault(false))
                .OrderByDescending(u => u.IsNotificationViewed)
                .ThenByDescending(u => u.Notification.NotificationCreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            var items = pagedQuery.AsEnumerable();

            return (items, totalRecords);
        }

        public async Task<bool> IsAny((int, Guid) id)
        {
            var (notificationId, userId) = id;
            return await _context.NotificationUsers
                .AnyAsync(nu => nu.NotificationId == notificationId && nu.UserId == userId);
        }

        public async Task<bool> SaveAsync()
        {
            try
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public bool Update(NotificationUser entity)
        {
            try
            {
                _context.NotificationUsers.Update(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

