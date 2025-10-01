using AuthService.Repositories.IRepositories; 
using NotificationService.Models;

namespace NotificationService.Repositories.IRepositories
{
    public interface INotificationRepository : IGenericRepository<int, Notification>
    {
        
    }
}