using NotificationService.Models;

namespace NotificationService.Repositories.IRepositories
{
    public interface INotificationUserRepository : IGenericRepository<(int , Guid), NotificationUser>
    {
        public Task<(IEnumerable<NotificationUser> Items, int TotalRecords, int UnviewCount)> GetPagedByUserIdAsync(
        Guid userId, 
        int pageNumber,
        int pageSize,
        string? searchQuery);
    }
}
