using OrderService.Models;

namespace OrderService.Repositories.Interfaces
{
    public interface IOrderVariationSingleRepo : IGenericRepository<(Guid, int),OrderVariationSingle>
    {
    }
}
