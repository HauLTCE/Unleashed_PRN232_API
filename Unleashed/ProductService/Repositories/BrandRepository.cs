using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.DTOs.Common;
using ProductService.Models;
using ProductService.Repositories.Extensions;
using ProductService.Repositories.IRepositories;

namespace ProductService.Repositories
{
    public class BrandRepository : IBrandRepository
    {
        private readonly ProductDbContext _context;

        public BrandRepository(ProductDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Brand>> GetPagedAsync(PaginationParams pagination)
        {
            var query = _context.Brands.AsQueryable();

            if (!string.IsNullOrEmpty(pagination.Search))
            {
                query = query.Where(b => b.BrandName.Contains(pagination.Search));
            }

            query = query.OrderBy(b => b.BrandName);

            return await query.ToPagedResultAsync(pagination.PageNumber, pagination.PageSize);
        }

        public async Task<Brand?> GetByIdAsync(int id)
            => await _context.Brands.FindAsync(id);

        public async Task<Brand> CreateAsync(Brand brand)
        {
            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();
            return brand;
        }

        public async Task<Brand?> UpdateAsync(Brand brand)
        {
            _context.Brands.Update(brand);
            await _context.SaveChangesAsync();
            return brand;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null) return false;

            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
            => await _context.Brands.AnyAsync(b => b.BrandId == id);
    }
}
