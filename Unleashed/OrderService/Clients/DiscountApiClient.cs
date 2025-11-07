using OrderService.Clients.IClients;
using OrderService.DTOs.ResponesDtos;
using System.Net.Http.Json;

namespace OrderService.Clients
{
    public class DiscountApiClient : IDiscountApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DiscountApiClient> _logger;

        public DiscountApiClient(HttpClient httpClient, ILogger<DiscountApiClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<DiscountResponse?> Get(int? discountId)
        {
            if (!discountId.HasValue) return null;

            try
            {
                // Gọi tới endpoint GET api/discounts/{id} đã có sẵn
                var response = await _httpClient.GetAsync($"api/discounts/{discountId.Value}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<DiscountResponse>();
                }
                _logger.LogWarning("Không thể lấy thông tin cho DiscountId {DiscountId}. Status: {StatusCode}", discountId, response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gọi Get Discount API cho ID: {DiscountId}", discountId);
                return null;
            }
        }

        public async Task UseDiscount(int? discountId)
        {
            if (!discountId.HasValue) return;

            try
            {
                // Gọi tới endpoint PUT api/discounts/{id}/use mới tạo
                var response = await _httpClient.PutAsync($"api/discounts/{discountId.Value}/use", null);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Ghi nhận lượt sử dụng cho DiscountId {DiscountId} thất bại. Status: {StatusCode}", discountId, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gọi Use Discount API cho ID: {DiscountId}", discountId);
            }
        }
    }
}