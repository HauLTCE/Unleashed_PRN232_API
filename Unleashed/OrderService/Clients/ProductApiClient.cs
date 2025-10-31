    using OrderService.DTOs.ResponesDtos;
    using ProductService.Clients.IClients;
    using System.Collections.Generic;
    using System.Net.Http.Json; // <-- Important: for GetFromJsonAsync

    namespace OrderService.Clients
    {
        public class ProductApiClient : IProductApiClient
        {
            private readonly HttpClient _httpClient;
            private readonly ILogger<ProductApiClient> _logger;

            public ProductApiClient(HttpClient httpClient, ILogger<ProductApiClient> logger)
            {
                _httpClient = httpClient; // The factory provides this
                _logger = logger;
            }


        public async Task<IEnumerable<VariationResponseDto>> GetDetailsByIdsAsync(IEnumerable<int> variationIds)
        {
            // 1. Handle empty list to avoid a pointless API call
            if (variationIds == null || !variationIds.Any())
            {
                return [];
            }

            try
            {
                // 2. Use PostAsJsonAsync to send the list of IDs in the request body.
                // This is the standard "batch-get" pattern in microservices.
                // It expects an endpoint like: [HttpPost("batch")]
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/variations/batch", variationIds);

                // 3. Check for success (2xx)
                response.EnsureSuccessStatusCode();

                // 4. Deserialize the response (which should be a list of DTOs)
                var variations = await response.Content.ReadFromJsonAsync<IEnumerable<VariationResponseDto>>();

                return variations ?? [];
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP batch request failed when calling Variation API. Status: {StatusCode}", ex.StatusCode);
                // If the batch call fails, we cannot proceed with the order.
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during Variation API batch request.");
                throw;
            }
        }
    }
    }
