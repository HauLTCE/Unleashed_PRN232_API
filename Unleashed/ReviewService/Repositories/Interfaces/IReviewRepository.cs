using ReviewService.DTOs.Internal;
using ReviewService.Helpers;
using ReviewService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReviewService.Repositories.Interfaces
{
    public interface IReviewRepository
    {
        Task<IEnumerable<Review>> GetAllAsync();
        Task<Review?> GetByIdAsync(int id);
        Task<Review> AddAsync(Review review);
        Task UpdateAsync(Review review);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> ExistsByProductAndOrderAndUserAsync(Guid productId, Guid orderId, Guid userId);
        Task<PagedResult<Review>> GetTopLevelReviewsByProductIdAsync(Guid productId, int page, int size);
        Task<PagedResult<Comment>> GetChildCommentsPaginatedAsync(int commentId, int page, int size);
        Task<PagedResult<Review>> GetReviewsByUserIdAsync(Guid userId, int page, int size);
        Task<PagedResult<Review>> GetRecentReviewsAsync(int page, int size);
        Task<bool> HasUserReviewedProductAsync(Guid productId, Guid userId);
        Task<IEnumerable<ProductRatingSummaryDto>> GetProductRatingSummariesAsync(IEnumerable<Guid> productIds);
    }
}