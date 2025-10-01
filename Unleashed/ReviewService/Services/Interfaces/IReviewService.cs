using ReviewService.DTOs.Review;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReviewService.Services.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDto>> GetAllReviewsAsync();
        Task<ReviewDto?> GetReviewByIdAsync(int id);
        Task<ReviewDto> CreateReviewAsync(CreateReviewDto reviewDto);
        Task<bool> UpdateReviewAsync(int id, UpdateReviewDto reviewDto);
        Task<bool> DeleteReviewAsync(int id);
    }
}