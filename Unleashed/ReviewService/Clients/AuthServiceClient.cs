using ReviewService.Clients.Interfaces;
using ReviewService.DTOs.External;

namespace ReviewService.Clients
{
    public class AuthServiceClient : IAuthServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthServiceClient> _logger;

        public AuthServiceClient(HttpClient httpClient, ILogger<AuthServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
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