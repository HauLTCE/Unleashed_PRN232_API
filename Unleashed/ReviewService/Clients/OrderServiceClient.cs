using ReviewService.Clients.Interfaces;
using ReviewService.DTOs.External;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ReviewService.Clients
{
    public class OrderServiceClient : IOrderServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OrderServiceClient> _logger;

        public OrderServiceClient(HttpClient httpClient, ILogger<OrderServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<OrderDto?> GetOrderByIdAsync(Guid orderId)
        {
            try
            {
                var order = await _httpClient.GetFromJsonAsync<OrderDto>($"api/order/{orderId}");
                return order;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed while fetching order with ID {OrderId}", orderId);
                return null;
            }
        }

        public async Task<List<OrderDto>?> GetEligibleOrdersForReviewAsync(Guid userId, Guid productId)
        {
            try
            {
                var requestUri = $"api/order/user/{userId}/eligible-for-review?productId={productId}";
                var orders = await _httpClient.GetFromJsonAsync<List<OrderDto>>(requestUri);
                return orders ?? new List<OrderDto>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed fetching eligible orders for user {UserId}, product {ProductId}", userId, productId);
                return null;
            }
        }
    }
}