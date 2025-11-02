using OrderService.Clients.IClients;
using OrderService.DTOs.ResponesDtos;

namespace OrderService.Clients
{
    public class DiscountApiClient : IDiscountApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<InventoryApiClient> _logger;

        public DiscountApiClient(HttpClient httpClient, ILogger<InventoryApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        public Task<DiscountResponse> Get(int? discountId)
        {
            throw new NotImplementedException();
        }

        public Task UseDiscount(int? discountId)
        {
            throw new NotImplementedException();
        }
    }
}
