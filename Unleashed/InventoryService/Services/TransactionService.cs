using AutoMapper;
using InventoryService.Data;
using InventoryService.DTOs.Transaction;
using InventoryService.Models;
using InventoryService.Repositories.Interfaces;
using InventoryService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryService.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IStockVariationRepository _stockVariationRepository;
        private readonly InventoryDbContext _context; // For atomicity
        private readonly IMapper _mapper;

        public TransactionService(
            ITransactionRepository transactionRepository,
            IStockVariationRepository stockVariationRepository,
            InventoryDbContext context,
            IMapper mapper)
        {
            _transactionRepository = transactionRepository;
            _stockVariationRepository = stockVariationRepository;
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TransactionDto>> GetAllTransactionsAsync()
        {
            var transactions = await _transactionRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<TransactionDto>>(transactions);
        }

        public async Task<TransactionDto?> GetTransactionByIdAsync(int id)
        {
            var transaction = await _transactionRepository.GetByIdAsync(id);
            return _mapper.Map<TransactionDto>(transaction);
        }

        public async Task<TransactionDto?> CreateTransactionAsync(CreateTransactionDto transactionDto)
        {
            var stockVariation = await _stockVariationRepository.GetByIdAsync(transactionDto.StockId.Value, transactionDto.VariationId.Value);
            if (stockVariation == null)
            {
                // Or create a new stock variation if that's the desired business logic
                return null;
            }

            var transactionEntity = _mapper.Map<Transaction>(transactionDto);
            transactionEntity.TransactionDate = DateTimeOffset.UtcNow;

            // TODO: Implement logic based on TransactionTypeId
            // Assuming 1 = IN, 2 = OUT for this example
            if (transactionDto.TransactionTypeId == 1) // IN
            {
                stockVariation.StockQuantity += transactionDto.TransactionQuantity;
            }
            else if (transactionDto.TransactionTypeId == 2) // OUT
            {
                if (stockVariation.StockQuantity < transactionDto.TransactionQuantity)
                {
                    // Not enough stock, handle this error appropriately
                    return null;
                }
                stockVariation.StockQuantity -= transactionDto.TransactionQuantity;
            }

            await _transactionRepository.AddAsync(transactionEntity);
            await _stockVariationRepository.UpdateAsync(stockVariation);

            var newTransaction = await _transactionRepository.AddAsync(transactionEntity);
            return _mapper.Map<TransactionDto>(newTransaction);
        }

        public async Task<bool> UpdateTransactionAsync(int id, UpdateTransactionDto transactionDto)
        {
            // Note: A robust implementation would reverse the old transaction's stock impact 
            // and apply the new one. This is a simplified version.
            if (!await _transactionRepository.ExistsAsync(id))
            {
                return false;
            }

            var transactionEntity = _mapper.Map<Transaction>(transactionDto);
            transactionEntity.TransactionId = id; // Ensure the ID is set for the update

            await _transactionRepository.UpdateAsync(transactionEntity);
            return true;
        }

        public async Task<bool> DeleteTransactionAsync(int id)
        {
            var transactionToDelete = await _transactionRepository.GetByIdAsync(id);
            if (transactionToDelete == null)
            {
                return false;
            }

            var stockVariation = await _stockVariationRepository.GetByIdAsync(transactionToDelete.StockId.Value, transactionToDelete.VariationId.Value);
            if (stockVariation != null)
            {
                // Reverse the stock change
                if (transactionToDelete.TransactionTypeId == 1) // IN
                {
                    stockVariation.StockQuantity -= transactionToDelete.TransactionQuantity;
                }
                else if (transactionToDelete.TransactionTypeId == 2) // OUT
                {
                    stockVariation.StockQuantity += transactionToDelete.TransactionQuantity;
                }
                await _stockVariationRepository.UpdateAsync(stockVariation);
            }

            await _transactionRepository.DeleteAsync(id);
            return true;
        }
    }
}