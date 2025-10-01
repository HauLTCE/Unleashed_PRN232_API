using InventoryService.Data;
using InventoryService.Models;
using InventoryService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryService.Repositories
{
    public class TransactionTypeRepository : ITransactionTypeRepository
    {
        private readonly InventoryDbContext _context;

        public TransactionTypeRepository(InventoryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TransactionType>> GetAllAsync()
        {
            return await _context.TransactionTypes.ToListAsync();
        }

        public async Task<TransactionType?> GetByIdAsync(int id)
        {
            return await _context.TransactionTypes.FindAsync(id);
        }

        public async Task<TransactionType> AddAsync(TransactionType transactionType)
        {
            _context.TransactionTypes.Add(transactionType);
            await _context.SaveChangesAsync();
            return transactionType;
        }

        public async Task UpdateAsync(TransactionType transactionType)
        {
            _context.Entry(transactionType).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var transactionType = await _context.TransactionTypes.FindAsync(id);
            if (transactionType != null)
            {
                _context.TransactionTypes.Remove(transactionType);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.TransactionTypes.AnyAsync(e => e.TransactionTypeId == id);
        }
    }
}