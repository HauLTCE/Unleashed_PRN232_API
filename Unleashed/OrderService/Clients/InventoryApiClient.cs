using OrderService.Clients.IClients;
using OrderService.DTOs.ResponesDtos;

namespace OrderService.Clients
{
    public class InventoryApiClient : IInventoryApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<InventoryApiClient> _logger;

        public InventoryApiClient(HttpClient httpClient, ILogger<InventoryApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;      
        }

        public async Task<IEnumerable<InventoryResponseDto>> GetStockByIdsAsync(IEnumerable<int> variationIds)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/StockVariations/get-stock-by-ids", variationIds);

                // 2. Check if the call was successful
                if (response.IsSuccessStatusCode)
                {
                    // 3. Read and deserialize the JSON response from the body
                    var stockLevels = await response.Content.ReadFromJsonAsync<IEnumerable<InventoryResponseDto>>();
                    return stockLevels ?? [];
                }
                else
                {
                    // 4. Handle errors
                    _logger.LogError("Failed to get stock from Inventory API. Status: {StatusCode}", response.StatusCode);
                    return Enumerable.Empty<InventoryResponseDto>();
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request error while getting stock.");
                throw; // Re-throw or handle as appropriate
            }
        }
    }
}
