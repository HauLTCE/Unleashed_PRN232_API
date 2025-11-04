using ReviewService.Clients.Interfaces;
using ReviewService.DTOs.External;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ReviewService.Clients
{
    public class NotificationServiceClient : INotificationServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<NotificationServiceClient> _logger;
        private readonly string _apiKey = "AAHNCO198486";

        public NotificationServiceClient(HttpClient httpClient, ILogger<NotificationServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _httpClient.DefaultRequestHeaders.Add("Key", _apiKey);
        }

        public async Task<bool> CreateNotificationForUsersAsync(CreateNotificationForUsersRequestDto createDto)
        {
            if (createDto == null)
            {
                throw new ArgumentNullException(nameof(createDto));
            }

            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/notifications/system", createDto);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed during targeted notification creation for users.");
                return false;
            }
        }
    }
}