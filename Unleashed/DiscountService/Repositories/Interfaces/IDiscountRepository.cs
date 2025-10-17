using DiscountService.Models;

namespace DiscountService.Repositories.Interfaces
{
    public interface IDiscountRepository : IGenericRepository<int, Discount>
    {
        // Thêm phương thức tìm theo code
        Task<Discount?> FindByCodeAsync(string code);

        // Các phương thức cho việc cập nhật trạng thái tự động
        Task<List<Discount>> GetDiscountsToActivateAsync(int inactiveStatusId, DateTimeOffset now);
        Task<List<Discount>> GetActiveDiscountsWithUsageLimitAsync(int activeStatusId);
        Task<List<Discount>> GetDiscountsToExpireAsync(int expiredStatusId, DateTimeOffset now);
    }
}
