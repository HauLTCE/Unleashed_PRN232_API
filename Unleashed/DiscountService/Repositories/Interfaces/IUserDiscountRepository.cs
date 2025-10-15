using DiscountService.Models;

namespace DiscountService.Repositories.Interfaces
{
    public interface IUserDiscountRepository : IGenericRepository<(Guid,int), UserDiscount>
    {
    }
}
