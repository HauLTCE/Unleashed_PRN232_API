using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.DTOs.OtherDTOs;
using ProductService.Models;
using ProductService.Repositories.IRepositories;

namespace ProductService.Repositories
{
    public class ColorRepository : IColorRepository
    {
        private readonly ProductDbContext _context;

        public ColorRepository(ProductDbContext context)
        {
            _context = context;
        }

        public async Task<List<Color>> GetAllAsync()
        {
            return await _context.Colors
                .AsNoTracking()
                .OrderBy(c => c.ColorName)
                .ToListAsync();
        }

        // Lấy màu đang xuất hiện trong variation; optional: chỉ sản phẩm Active
        public async Task<List<Color>> GetAvailableAsync(bool onlyActiveProducts = false)
        {
            var query = _context.Colors
                .AsNoTracking()
                .Where(c => _context.Variations.Any(v => v.ColorId == c.ColorId
                    && (!onlyActiveProducts ||
                        (_context.Products.Any(p => p.ProductId == v.ProductId
                            && p.ProductStatus != null
                            && p.ProductStatus.ProductStatusName == "Active")))))
                .OrderBy(c => c.ColorName);

            return await query.ToListAsync();
        }
        public async Task<Color?> GetByIdAsync(int id)
         => await _context.Colors.FindAsync(id);
        public async Task<bool> ExistsAsync(int id) => await _context.Colors.AnyAsync(x => x.ColorId == id);

    }
}
