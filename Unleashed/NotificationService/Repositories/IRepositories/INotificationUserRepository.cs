using NotificationService.Models;

namespace NotificationService.Repositories.IRepositories
{
    public interface INotificationUserRepository : IGenericRepository<(int , Guid), NotificationUser>
    {
    }
}
