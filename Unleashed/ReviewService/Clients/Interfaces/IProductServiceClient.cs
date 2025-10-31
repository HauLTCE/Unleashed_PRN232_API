using ReviewService.DTOs.External;

namespace ReviewService.Clients.Interfaces
{
    public interface IProductServiceClient
    {
        Task<IEnumerable<ProductSummaryDto>> GetProductsByIdsAsync(IEnumerable<Guid> productIds);
    }
}