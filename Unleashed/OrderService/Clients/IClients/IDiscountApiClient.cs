using OrderService.DTOs.ResponesDtos;

namespace OrderService.Clients.IClients
{
    public interface IDiscountApiClient
    {
        Task<DiscountResponse> Get(int? discountId);
        Task UseDiscount(int? discountId);
    }
}
