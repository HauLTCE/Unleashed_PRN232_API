using NotificationService.Clients.IClients;
using NotificationService.DTOs.External;

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
        public async Task<UserDto?> GetUserByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return null;
            }

            try
            {
                return await _httpClient.GetFromJsonAsync<UserDto>($"api/users/by-username/{username}");
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("User with username '{Username}' not found in AuthService.", username);
                }
                else
                {
                    _logger.LogError(ex, "HTTP request failed while fetching user '{Username}'.", username);
                }
                return null;
            }
        }

        public async Task<IEnumerable<UserDto>> GetUsersByIdsAsync(IEnumerable<Guid> userIds)
        {
            if (userIds == null || !userIds.Any())
            {
                return Enumerable.Empty<UserDto>();
            }

            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/users/batch", userIds);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<IEnumerable<UserDto>>() ?? Enumerable.Empty<UserDto>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed during batch fetch of users.");
                return Enumerable.Empty<UserDto>();
            }
        }
    }
}
