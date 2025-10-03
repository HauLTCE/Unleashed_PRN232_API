using InventoryService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryService.Repositories.Interfaces
{
    public interface IStockRepository
    {
        Task<IEnumerable<Stock>> GetAllAsync();
        Task<Stock?> GetByIdAsync(int id);
        Task<Stock> AddAsync(Stock stock);
        Task UpdateAsync(Stock stock);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}