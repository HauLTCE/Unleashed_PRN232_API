using InventoryService.Data;
using InventoryService.Models;
using InventoryService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryService.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly InventoryDbContext _context;

        public StockRepository(InventoryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Stock>> GetAllAsync()
        {
            return await _context.Stocks.ToListAsync();
        }

        public async Task<Stock?> GetByIdAsync(int id)
        {
            return await _context.Stocks.FindAsync(id);
        }

        public async Task<Stock> AddAsync(Stock stock)
        {
            _context.Stocks.Add(stock);
            await _context.SaveChangesAsync();
            return stock;
        }

        public async Task UpdateAsync(Stock stock)
        {
            _context.Entry(stock).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var stock = await _context.Stocks.FindAsync(id);
            if (stock != null)
            {
                _context.Stocks.Remove(stock);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Stocks.AnyAsync(e => e.StockId == id);
        }
    }
}