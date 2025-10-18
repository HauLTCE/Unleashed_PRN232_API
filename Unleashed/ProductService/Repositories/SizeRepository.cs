using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Models;
using ProductService.Repositories.IRepositories;

namespace ProductService.Repositories
{
    public class SizeRepository : ISizeRepository
    {
        private readonly ProductDbContext _context;

        public SizeRepository(ProductDbContext context)
        {
            _context = context;
        }
        public async Task<List<Size>> GetAllAsync()
        {
            return await _context.Sizes
                .AsNoTracking()
                .OrderBy(s => s.SizeName)
                .ToListAsync();
        }

        public async Task<List<Size>> GetAvailableAsync(bool onlyActiveProducts = false)
        {
            var query = _context.Sizes
                .AsNoTracking()
                .Where(s => _context.Variations.Any(v => v.SizeId == s.SizeId
                    && (!onlyActiveProducts ||
                        (_context.Products.Any(p => p.ProductId == v.ProductId
                            && p.ProductStatus != null
                            && p.ProductStatus.ProductStatusName == "Active")))))
                .OrderBy(s => s.SizeName);

            return await query.ToListAsync();
        }

        public async Task<Size?> GetByIdAsync(int id) => await _context.Sizes.FindAsync(id);
        public async Task<bool> ExistsAsync(int id) => await _context.Sizes.AnyAsync(x => x.SizeId == id);
    }
}
