using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using InventoryService.Data;
using InventoryService.Models;
using InventoryService.Repositories.Interfaces;
using InventoryService.Specifications;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryService.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly InventoryDbContext _context;
        private readonly ISpecificationEvaluator _specificationEvaluator = new SpecificationEvaluator();

        public TransactionRepository(InventoryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Transaction>> GetAllAsync()
        {
            return await _context.Transactions.ToListAsync();
        }

        public async Task<Transaction?> GetByIdAsync(int id)
        {
            return await _context.Transactions.FindAsync(id);
        }

        public async Task<Transaction> AddAsync(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            // Note: SaveChangesAsync is handled in the service layer
            return transaction;
        }

        public async Task UpdateAsync(Transaction transaction)
        {
            _context.Entry(transaction).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Transactions.AnyAsync(e => e.TransactionId == id);
        }

        public async Task<List<Transaction>> ListAsync(ISpecification<Transaction> spec)
        {
            var queryResult = _specificationEvaluator.GetQuery(_context.Set<Transaction>().AsQueryable(), spec);
            return await queryResult.ToListAsync();
        }

        public async Task<int> CountAsync(ISpecification<Transaction> spec)
        {
            var queryResult = _specificationEvaluator.GetQuery(_context.Set<Transaction>().AsQueryable(), spec);
            return await queryResult.CountAsync();
        }

    }
}