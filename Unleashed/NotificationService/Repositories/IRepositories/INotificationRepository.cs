using NotificationService.Models;

namespace NotificationService.Repositories.IRepositories
{
    public interface INotificationRepository : IGenericRepository<int, Notification>
    {
        Task<(IEnumerable<Notification> entities, int totalCount)> GetPagedAsync(
       int pageNumber,
       int pageSize,
       string? searchQuery,
       bool? isDraft);
    }
}