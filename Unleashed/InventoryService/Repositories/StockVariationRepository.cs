using InventoryService.Data;
using InventoryService.Models;
using InventoryService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Repositories
{
    public class StockVariationRepository : IStockVariationRepository
    {
        private readonly InventoryDbContext _context;

        public StockVariationRepository(InventoryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StockVariation>> GetAllAsync()
        {
            return await _context.StockVariations.ToListAsync();
        }

        public async Task<StockVariation?> GetByIdAsync(int stockId, int variationId)
        {
            return await _context.StockVariations.FindAsync(stockId, variationId);
        }

        public async Task<List<StockVariation>> GetByIdsAsync(IEnumerable<int> variationIds)
        {
            return await _context.StockVariations
                .Where(sv => variationIds.Contains(sv.VariationId))
                .ToListAsync();
        }

        public async Task<StockVariation> AddAsync(StockVariation stockVariation)
        {
            _context.StockVariations.Add(stockVariation);
            await _context.SaveChangesAsync();
            return stockVariation;
        }

        public async Task UpdateAsync(StockVariation stockVariation)
        {
            _context.Entry(stockVariation).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRangeAsync(IEnumerable<StockVariation> stockVariations)
        {
            _context.StockVariations.UpdateRange(stockVariations);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(StockVariation stockVariation)
        {
            _context.StockVariations.Remove(stockVariation);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int stockId, int variationId)
        {
            return await _context.StockVariations.AnyAsync(e => e.StockId == stockId && e.VariationId == variationId);
        }
    }
}