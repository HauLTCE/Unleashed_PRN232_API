using DiscountService.Models;

namespace DiscountService.Repositories.Interfaces
{
    public interface IUserDiscountRepository : IGenericRepository<(Guid,int), UserDiscount>
    {

        Task<List<UserDiscount>> FindByUserIdAsync(Guid userId);
        Task<List<int>> FindDiscountIdsByUserIdAsync(Guid userId);
        Task<UserDiscount?> FindByUserIdAndDiscountIdAsync(Guid userId, int discountId);
        Task<bool> ExistsAsync(Guid userId, int discountId);
        Task AddRangeAsync(IEnumerable<UserDiscount> userDiscounts);
    }
}
