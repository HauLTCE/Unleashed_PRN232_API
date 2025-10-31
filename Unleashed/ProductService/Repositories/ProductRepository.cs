using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.DTOs.CategoryDTOs;
using ProductService.DTOs.Common;
using ProductService.DTOs.ProductDTOs;
using ProductService.Models;
using ProductService.Repositories.Extensions;
using ProductService.Repositories.IRepositories;

namespace ProductService.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDbContext _context;

        public ProductRepository(ProductDbContext context)
        {
            _context = context;
        }
        public async Task<PagedResult<Product>> GetPagedAsync(PaginationParams pagination)
        {
            var query = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.ProductStatus)
                .Include(p => p.Variations)
                    .ThenInclude(v => v.Color)
                .Include(p => p.Variations)
                    .ThenInclude(v => v.Size)
                .AsQueryable();

            // Search
            if (!string.IsNullOrEmpty(pagination.Search))
            {
                query = query.Where(p =>
                    p.ProductName.Contains(pagination.Search) ||
                    p.ProductCode.Contains(pagination.Search));
            }

            query = query.OrderByDescending(p => p.ProductCreatedAt);

            return await query.ToPagedResultAsync(pagination.PageNumber, pagination.PageSize);
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.ProductStatus)
                .Include(p => p.Variations)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<Product> CreateAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product?> UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Products.AnyAsync(p => p.ProductId == id);
        }

        public async Task<bool> DeleteByProductIdAsync(Guid productId)
        {
            var list = await _context.Variations.Where(v => v.ProductId == productId).ToListAsync();
            if (list.Count == 0) return false;
            _context.Variations.RemoveRange(list);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<Product?> GetByIdWithFirstVariationAsync(Guid productId)
        {
            return await _context.Products
                .Include(p => p.Variations.OrderBy(v => v.VariationId))
                .FirstOrDefaultAsync(p => p.ProductId == productId);
        }
        // Phương thức bắt đầu giao dịch
        public async Task BeginTransactionAsync()
        {
            await _context.Database.BeginTransactionAsync();
        }

        // Phương thức lưu các thay đổi trong giao dịch
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<Product?> GetByProductNameAsync(string productName)
        {
            return await _context.Products
                                 .FirstOrDefaultAsync(p => p.ProductName.Equals(productName, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<Product>> GetByIdsAsync(IEnumerable<Guid> ids)
        {
            if (ids == null || !ids.Any())
            {
                return Enumerable.Empty<Product>();
            }

            return await _context.Products
                .Include(p => p.Variations.OrderBy(v => v.VariationId))
                .Where(p => ids.Contains(p.ProductId))
                .ToListAsync();
        }
    }
}
