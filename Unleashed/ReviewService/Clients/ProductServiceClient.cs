using ReviewService.Clients.Interfaces;
using ReviewService.DTOs.External;

namespace ReviewService.Clients
{
    public class ProductServiceClient : IProductServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProductServiceClient> _logger;

        public ProductServiceClient(HttpClient httpClient, ILogger<ProductServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductSummaryDto>> GetProductsByIdsAsync(IEnumerable<Guid> productIds)
        {
            if (productIds == null || !productIds.Any())
            {
                return Enumerable.Empty<ProductSummaryDto>();
            }

            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/products/batch", productIds);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<IEnumerable<ProductSummaryDto>>() ?? Enumerable.Empty<ProductSummaryDto>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed during batch fetch of products.");
                return Enumerable.Empty<ProductSummaryDto>();
            }
        }
    }
}