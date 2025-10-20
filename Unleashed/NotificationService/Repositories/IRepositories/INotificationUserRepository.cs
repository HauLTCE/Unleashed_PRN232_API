using NotificationService.Models;

namespace NotificationService.Repositories.IRepositories
{
    public interface INotificationUserRepository : IGenericRepository<(int , Guid), NotificationUser>
    {
        public Task<(IEnumerable<NotificationUser>, int)> GetPagedByUserIdAsync(
        Guid userId, 
        int pageNumber,
        int pageSize,
        string? searchQuery);
    }
}
