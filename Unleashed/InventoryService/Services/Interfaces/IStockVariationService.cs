using InventoryService.DTOs.External;
using InventoryService.DTOs.StockVariation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryService.Services.Interfaces
{
    public interface IStockVariationService
    {
        Task<IEnumerable<StockVariationDto>> GetAllStockVariationsAsync();
        Task<Inventory_OrderDto?> GetStockByVariationIdAsync(int variationId);
        Task<IEnumerable<Inventory_OrderDto?>> GetStockByVariationIdsAsync(List<int> variationId);
        Task<StockVariationDto?> GetStockVariationByIdAsync(int stockId, int variationId);
        Task<StockVariationDto?> CreateStockVariationAsync(CreateStockVariationDto stockVariationDto);
        Task<bool> UpdateStockVariationAsync(int stockId, int variationId, UpdateStockVariationDto stockVariationDto);
        Task<bool> DeleteStockVariationAsync(int stockId, int variationId);
        Task<IEnumerable<StockVariationDto>> GetStockVariationsByStockIdAsync(int stockId);
        Task DecreaseStocksAsync(List<Order_InventoryDto> orderList);
    }
}