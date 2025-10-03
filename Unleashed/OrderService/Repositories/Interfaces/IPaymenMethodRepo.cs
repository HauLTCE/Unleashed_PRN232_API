using OrderService.Models;

namespace OrderService.Repositories.Interfaces
{
    public interface IPaymenMethodRepo : IGenericRepository<int, PaymentMethod>
    {
    }
}
