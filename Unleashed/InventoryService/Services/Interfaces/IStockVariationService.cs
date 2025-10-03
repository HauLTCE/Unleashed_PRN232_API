using InventoryService.DTOs.StockVariation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryService.Services.Interfaces
{
    public interface IStockVariationService
    {
        Task<IEnumerable<StockVariationDto>> GetAllStockVariationsAsync();
        Task<StockVariationDto?> GetStockVariationByIdAsync(int stockId, int variationId);
        Task<StockVariationDto?> CreateStockVariationAsync(CreateStockVariationDto stockVariationDto);
        Task<bool> UpdateStockVariationAsync(int stockId, int variationId, UpdateStockVariationDto stockVariationDto);
        Task<bool> DeleteStockVariationAsync(int stockId, int variationId);
    }
}