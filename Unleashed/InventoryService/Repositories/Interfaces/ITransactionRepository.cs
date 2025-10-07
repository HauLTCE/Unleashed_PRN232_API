using Ardalis.Specification;
using InventoryService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryService.Repositories.Interfaces
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<Transaction>> GetAllAsync();
        Task<Transaction?> GetByIdAsync(int id);
        Task<Transaction> AddAsync(Transaction transaction);
        Task UpdateAsync(Transaction transaction);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<List<Transaction>> ListAsync(ISpecification<Transaction> spec);
        Task<int> CountAsync(string? searchTerm, string? dateFilter);

    }
}