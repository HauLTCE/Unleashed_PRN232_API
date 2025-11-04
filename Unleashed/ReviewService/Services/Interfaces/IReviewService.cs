using ReviewService.DTOs.Internal;
using ReviewService.DTOs.Review;
using ReviewService.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReviewService.Services.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDto>> GetAllReviewsAsync();
        Task<ReviewDto?> GetReviewByIdAsync(int id);
        Task<ReviewDto> CreateReviewAsync(CreateReviewDto reviewDto, Guid currentUserId);
        Task<bool> UpdateReviewAsync(int id, UpdateReviewDto reviewDto);
        Task<bool> DeleteReviewAsync(int id);

        Task<PagedResult<ProductReviewDto>> GetAllReviewsByProductIdAsync(Guid productId, int page, int size, Guid? currentUserId);
        Task<PagedResult<ProductReviewDto>> GetRepliesForCommentAsync(int commentId, int page, int size);
        Task<PagedResult<UserReviewHistoryDto>> GetReviewsByUserIdAsync(Guid userId, int page, int size);
        Task<bool> GetReviewEligibilityAsync(Guid productId, Guid userId);
        Task<bool> CheckReviewExistsAsync(Guid productId, Guid orderId, Guid userId);
        Task<PagedResult<DashboardReviewDto>> GetDashboardReviewsAsync(int page, int size);
    }
}