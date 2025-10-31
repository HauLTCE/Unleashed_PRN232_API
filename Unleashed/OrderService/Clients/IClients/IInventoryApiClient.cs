using OrderService.DTOs.ResponesDtos;

namespace OrderService.Clients.IClients
{
    public interface IInventoryApiClient
    {
        Task<IEnumerable<InventoryResponseDto>> GetStockByIdsAsync(IEnumerable<int> variationIds);
    }
}

