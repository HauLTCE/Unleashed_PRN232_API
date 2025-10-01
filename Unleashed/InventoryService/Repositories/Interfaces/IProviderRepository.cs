using InventoryService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryService.Repositories.Interfaces
{
    public interface IProviderRepository
    {
        Task<IEnumerable<Provider>> GetAllAsync();
        Task<Provider?> GetByIdAsync(int id);
        Task<Provider> AddAsync(Provider provider);
        Task UpdateAsync(Provider provider);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}