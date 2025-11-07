using OrderService.Dtos;
using OrderService.DTOs.ResponesDtos;
using OrderService.Models;

namespace OrderService.Clients.IClients
{
    public interface IInventoryApiClient
    {
        Task<IEnumerable<InventoryResponseDto>> GetStockByIdsAsync(IEnumerable<int> variationIds);
        Task<bool> DecreaseStocksAsync(List<OrderVariation> list);
        Task<bool> ReturnStocksAsync(List<OrderVariation> list);
    }
}

