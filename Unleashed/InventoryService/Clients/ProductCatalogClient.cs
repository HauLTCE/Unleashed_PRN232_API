using InventoryService.Clients.Interfaces;
using InventoryService.DTOs.External;
using System.Text.Json;

namespace InventoryService.Clients
{
    public class ProductCatalogClient : IProductCatalogClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProductCatalogClient> _logger;

        public ProductCatalogClient(HttpClient httpClient, ILogger<ProductCatalogClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<VariationDto?> GetVariationByIdAsync(int variationId)
        {
            try
            {
                // TODO: FIX THIS SHIT SO THAT IT IS TRUE TO THE VARIATION API THINGY
                return await _httpClient.GetFromJsonAsync<VariationDto>($"api/variations/{variationId}");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed while fetching variation with ID {VariationId}.", variationId);
                return null;
            }
        }

        public async Task<IEnumerable<VariationDto>> GetVariationsByIdsAsync(IEnumerable<int> variationIds)
        {
            if (variationIds == null || !variationIds.Any())
            {
                return Enumerable.Empty<VariationDto>();
            }

            try
            {
                // TODO: FIX THIS SHIT SO THAT IT IS TRUE TO THE VARIATION API THINGY
                var response = await _httpClient.PostAsJsonAsync("api/variations/batch", variationIds);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                // deserialize manually to handle potential case-insensitivity in property names
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var variations = JsonSerializer.Deserialize<IEnumerable<VariationDto>>(content, options);

                return variations ?? Enumerable.Empty<VariationDto>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed during batch fetch of variations.");
                return Enumerable.Empty<VariationDto>(); //empty list on failure
            }
        }
    }
}
