using InventoryService.DTOs.Transaction;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryService.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync();
        Task<TransactionDto?> GetTransactionByIdAsync(int id);
        Task<TransactionDto?> CreateTransactionAsync(CreateTransactionDto transactionDto);
        Task<bool> UpdateTransactionAsync(int id, UpdateTransactionDto transactionDto);
        Task<bool> DeleteTransactionAsync(int id);
    }
}