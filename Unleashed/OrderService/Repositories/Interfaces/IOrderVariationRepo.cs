using OrderService.Models;

namespace OrderService.Repositories.Interfaces
{
    public interface IOrderVariationRepo : IGenericRepository<(Guid, int), Models.OrderVariation>
    {
    }
}
