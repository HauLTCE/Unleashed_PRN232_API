using InventoryService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryService.Repositories.Interfaces
{
    public interface ITransactionTypeRepository
    {
        Task<IEnumerable<TransactionType>> GetAllAsync();
        Task<TransactionType?> GetByIdAsync(int id);
        Task<TransactionType> AddAsync(TransactionType transactionType);
        Task UpdateAsync(TransactionType transactionType);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}