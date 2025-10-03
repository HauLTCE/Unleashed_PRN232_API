using InventoryService.DTOs.TransactionType;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryService.Services.Interfaces
{
    public interface ITransactionTypeService
    {
        Task<IEnumerable<TransactionTypeDto>> GetAllTransactionTypesAsync();
        Task<TransactionTypeDto?> GetTransactionTypeByIdAsync(int id);
        Task<TransactionTypeDto> CreateTransactionTypeAsync(CreateTransactionTypeDto transactionTypeDto);
        Task<bool> UpdateTransactionTypeAsync(int id, UpdateTransactionTypeDto transactionTypeDto);
        Task<bool> DeleteTransactionTypeAsync(int id);
    }
}