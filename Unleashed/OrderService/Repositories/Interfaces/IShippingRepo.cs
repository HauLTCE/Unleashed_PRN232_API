using OrderService.Models;

namespace OrderService.Repositories.Interfaces
{
    public interface IShippingRepo : IGenericRepository<int, ShippingMethod>
    {
    }
}
