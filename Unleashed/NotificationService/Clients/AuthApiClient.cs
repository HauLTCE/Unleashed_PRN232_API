using NotificationService.Clients.IClients;

namespace NotificationService.Clients
{
    public class AuthApiClient : IAuthApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthApiClient> _logger;

        public AuthApiClient(HttpClient httpClient, ILogger<AuthApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        public async Task<IEnumerable<Guid>> GetCustomers()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync("api/Users/get-customer");

                // 2. Check if the call was successful
                if (response.IsSuccessStatusCode)
                {
                    // 3. Read and deserialize the JSON response from the body
                    var customerIds = await response.Content.ReadFromJsonAsync<IEnumerable<Guid>>();
                    return customerIds ?? [];
                }
                else
                {
                    // 4. Handle errors
                    _logger.LogError("Failed to get stock from Auth API. Status: {StatusCode}", response.StatusCode);
                    return [];
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request error while getting customer.");
                throw; // Re-throw or handle as appropriate
            }
        }
    }
}
