using ReviewService.DTOs.External;

namespace ReviewService.Clients.Interfaces
{
    public interface IOrderServiceClient
    {
        Task<OrderDto> GetOrderByIdAsync(Guid orderId);

        Task<List<OrderDto>?> GetEligibleOrdersForReviewAsync(Guid userId, Guid productId);
    }
}