using InventoryService.Data;
using InventoryService.Models;
using InventoryService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryService.Repositories
{
    public class ProviderRepository : IProviderRepository
    {
        private readonly InventoryDbContext _context;

        public ProviderRepository(InventoryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Provider>> GetAllAsync()
        {
            return await _context.Providers.ToListAsync();
        }

        public async Task<Provider?> GetByIdAsync(int id)
        {
            return await _context.Providers.FindAsync(id);
        }

        public async Task<Provider> AddAsync(Provider provider)
        {
            _context.Providers.Add(provider);
            await _context.SaveChangesAsync();
            return provider;
        }

        public async Task UpdateAsync(Provider provider)
        {
            _context.Entry(provider).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var provider = await _context.Providers.FindAsync(id);
            if (provider != null)
            {
                _context.Providers.Remove(provider);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Providers.AnyAsync(e => e.ProviderId == id);
        }
    }
}