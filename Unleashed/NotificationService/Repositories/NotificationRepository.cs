using Microsoft.EntityFrameworkCore;
using NotificationService.Data; // Assuming your DbContext is in a 'Data' folder
using NotificationService.Models;
using NotificationService.Repositories.IRepositories;

namespace NotificationService.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly NotificationDbContext _context;

        public NotificationRepository(NotificationDbContext context)
        {
            _context = context;
        }
       
        public async Task<Notification?> FindAsync(int id)
        {
            return await _context.Notifications.FindAsync(id);
        }

        public async Task<bool> IsAny(int id)
        {
            return await _context.Notifications.AnyAsync(n => n.NotificationId == id);
        }

        public async Task<bool> CreateAsync(Notification entity)
        {
            try
            {
                entity.NotificationCreatedAt = DateTimeOffset.UtcNow;
                entity.NotificationUpdatedAt = DateTimeOffset.UtcNow;
                await _context.Notifications.AddAsync(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Update(Notification entity)
        {
            try
            {
                entity.NotificationUpdatedAt = DateTimeOffset.UtcNow;
                _context.Notifications.Update(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(Notification entity)
        {
            try
            {
                _context.Notifications.Remove(entity);
                return true;
            }
            catch
            {
                return false;
            }
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

        public async Task<IEnumerable<Notification>> All()
        {
            return await _context.Notifications.ToListAsync();
        }

        public async Task<(IEnumerable<Notification> entities, int totalCount)> GetPagedAsync(int pageNumber, int pageSize, string? searchQuery)
        {
            IQueryable<Notification> query = _context.Notifications.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var lowerCaseSearchTerm = searchQuery.Trim().ToLower();
                query = query.Where(n =>
                    n.NotificationTitle != null && n.NotificationTitle.ToLower().Contains(lowerCaseSearchTerm));
            }

            var totalRecords = await query.CountAsync();

            var pagedQuery = query
                .OrderByDescending(u => u.NotificationCreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            var items = pagedQuery.AsEnumerable();

            return (items, totalRecords);
        }
    }
}

