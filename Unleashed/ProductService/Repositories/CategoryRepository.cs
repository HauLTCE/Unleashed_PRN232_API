using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.DTOs.Common;
using ProductService.Models;
using ProductService.Repositories.Extensions;
using ProductService.Repositories.IRepositories;

namespace ProductService.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ProductDbContext _context;

        public CategoryRepository(ProductDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Category>> GetPagedAsync(PaginationParams pagination)
        {
            var query = _context.Categories.AsQueryable();

            if (!string.IsNullOrEmpty(pagination.Search))
            {
                query = query.Where(c => c.CategoryName.Contains(pagination.Search));
            }

            query = query.OrderBy(c => c.CategoryName);

            return await query.ToPagedResultAsync(pagination.PageNumber, pagination.PageSize);
        }

        public async Task<Category?> GetByIdAsync(int id)
            => await _context.Categories.FindAsync(id);

        public async Task<Category> CreateAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category?> UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
            => await _context.Categories.AnyAsync(c => c.CategoryId == id);
    }
}
