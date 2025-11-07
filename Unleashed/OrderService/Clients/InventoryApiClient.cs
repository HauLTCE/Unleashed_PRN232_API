using System.Net;
using System.Net.Http.Json;
using OrderService.Clients.IClients;
using OrderService.Dtos;
using OrderService.DTOs.ResponesDtos;
using OrderService.Models;

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
                var response = await _httpClient.PostAsJsonAsync(
                    "api/StockVariations/get-stock-by-ids",
                    variationIds);

                if (response.IsSuccessStatusCode)
                {
                    var stockLevels = await response.Content.ReadFromJsonAsync<IEnumerable<InventoryResponseDto>>();
                    return stockLevels ?? Enumerable.Empty<InventoryResponseDto>();
                }

                _logger.LogError(
                    "Inventory API get-stock-by-ids failed. Status: {StatusCode}. Message: {Message}",
                    response.StatusCode,
                    await response.Content.ReadAsStringAsync());

                return Enumerable.Empty<InventoryResponseDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Inventory API get-stock-by-ids");
                throw; // Re-throw to let upper layer handle it
            }
        }

        public async Task<bool> DecreaseStocksAsync(List<OrderVariation> list)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync(
                    "api/StockVariations/decrease-stocks",
                    list);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                // Log error details from response body
                var errorBody = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    "Inventory API decrease-stocks failed. Status: {StatusCode}. Response: {Body}",
                    response.StatusCode,
                    errorBody);

                // Optional: throw exception instead of returning false
                if (response.StatusCode == HttpStatusCode.BadRequest)
                    throw new InvalidOperationException(errorBody);

                if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new KeyNotFoundException(errorBody);

                throw new Exception($"Inventory API error {response.StatusCode}: {errorBody}");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request error while decreasing stocks.");
                throw;
            }
        }
        public async Task<bool> ReturnStocksAsync(List<OrderVariation> list)
        {
            try
            {
                // Gọi đến endpoint [HttpPost("return-stock")] trong TransactionsController
                var response = await _httpClient.PostAsJsonAsync(
                    "api/transactions/return-stock",
                    list);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                // Ghi log lỗi nếu không thành công
                var errorBody = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    "Inventory API return-stock failed. Status: {StatusCode}. Response: {Body}",
                    response.StatusCode,
                    errorBody);

                return false;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request error while returning stocks.");
                // Ném lại lỗi để lớp service có thể xử lý
                throw;
            }
        }
    }
}
