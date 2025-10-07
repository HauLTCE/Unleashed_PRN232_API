using InventoryService.Clients.Interfaces;
using InventoryService.DTOs.External;

namespace InventoryService.Clients
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

        public async Task<UserDto?> GetUserByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return null;
            }

            try
            {
                // TODO: FIX THIS SHIT SO THAT IT CAN CONNECT TO THE USERS API THINGY
                return await _httpClient.GetFromJsonAsync<UserDto>($"api/users/by-username?username={username}");
            }
            catch (HttpRequestException ex)
            {
                // not found = 404
                // not found is still valid?
                // idk i'm stupid mannn
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("User with username '{Username}' not found.", username);
                }
                else
                {
                    _logger.LogError(ex, "HTTP request failed while fetching user '{Username}'.", username);
                }
                return null;
            }
        }
    }
}
