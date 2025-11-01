namespace NotificationService.Clients.IClients
{
    public interface IAuthApiClient
    {
        Task<IEnumerable<Guid>> GetCustomers();
    }
}
