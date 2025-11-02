using InventoryService.Clients.Interfaces;
using InventoryService.DTOs.External;
using System.Text.Json;

namespace InventoryService.Clients
{
    public class ProductCatalogClient : IProductCatalogClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProductCatalogClient> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public ProductCatalogClient(HttpClient httpClient, ILogger<ProductCatalogClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<VariationDto?> GetVariationByIdAsync(int variationId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/variations/{variationId}");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Request to GetVariationByIdAsync failed with status code {StatusCode} for VariationId {VariationId}.", response.StatusCode, variationId);
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                var flatVariation = JsonSerializer.Deserialize<FlatVariationDto>(content, _jsonOptions);

                return flatVariation == null ? null : MapToNestedDto(flatVariation);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed while fetching variation with ID {VariationId}.", variationId);
                return null;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize response for variation with ID {VariationId}.", variationId);
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
                var response = await _httpClient.PostAsJsonAsync("api/variations/batch", variationIds);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var flatVariations = JsonSerializer.Deserialize<IEnumerable<FlatVariationDto>>(content, _jsonOptions);

                if (flatVariations == null) return Enumerable.Empty<VariationDto>();

                return flatVariations.Select(MapToNestedDto).ToList();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed during batch fetch of variations.");
                return Enumerable.Empty<VariationDto>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize the batch response from ProductService.");
                return Enumerable.Empty<VariationDto>();
            }
        }

        public async Task<IEnumerable<VariationDto>> SearchVariationsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return Enumerable.Empty<VariationDto>();
            }

            try
            {
                var response = await _httpClient.GetAsync($"api/variations?search={searchTerm}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var flatVariations = JsonSerializer.Deserialize<IEnumerable<FlatVariationDto>>(content, _jsonOptions);

                if (flatVariations == null) return Enumerable.Empty<VariationDto>();

                return flatVariations.Select(MapToNestedDto).ToList();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed during variation search for term: {SearchTerm}", searchTerm);
                return Enumerable.Empty<VariationDto>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize the search response from ProductService for term: {SearchTerm}", searchTerm);
                return Enumerable.Empty<VariationDto>();
            }
        }

        private VariationDto MapToNestedDto(FlatVariationDto flatDto)
        {
            return new VariationDto
            {
                VariationId = flatDto.VariationId,
                ProductId = flatDto.ProductId,
                SizeId = flatDto.SizeId,
                ColorId = flatDto.ColorId,
                VariationImage = flatDto.VariationImage,
                VariationPrice = flatDto.VariationPrice,
                Size = flatDto.Size,
                Color = flatDto.Color,
                Product = new ProductDto
                {
                    ProductName = flatDto.ProductName,
                    BrandName = flatDto.BrandName,
                    CategoryNames = flatDto.CategoryNames
                }
            };
        }
    }
}