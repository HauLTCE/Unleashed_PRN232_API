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

        public IQueryable<NotificationUser> All()
        {
            return _context.NotificationUsers.AsQueryable();
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

