using InventoryService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryService.Repositories.Interfaces
{
    public interface IStockVariationRepository
    {
        Task<IEnumerable<StockVariation>> GetAllAsync();
        Task<StockVariation?> GetByIdAsync(int stockId, int variationId);
        Task<List<StockVariation>> GetByVariationIdAsync(int variationId);
        Task<List<StockVariation>> GetByIdsAsync(IEnumerable<int> variationIds);
        Task<StockVariation> AddAsync(StockVariation stockVariation);
        Task UpdateAsync(StockVariation stockVariation);
        Task UpdateRangeAsync(IEnumerable<StockVariation> stockVariations);
        Task DeleteAsync(StockVariation stockVariation);
        Task<bool> ExistsAsync(int stockId, int variationId);
        Task<List<StockVariation>> GetByStockIdAsync(int stockId);
    }
}