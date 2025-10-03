using InventoryService.DTOs.Stock;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryService.Services.Interfaces
{
    public interface IStockService
    {
        Task<IEnumerable<StockDto>> GetAllStocksAsync();
        Task<StockDto?> GetStockByIdAsync(int id);
        Task<StockDto> CreateStockAsync(CreateStockDto stockDto);
        Task<bool> UpdateStockAsync(int id, UpdateStockDto stockDto);
        Task<bool> DeleteStockAsync(int id);
    }
}