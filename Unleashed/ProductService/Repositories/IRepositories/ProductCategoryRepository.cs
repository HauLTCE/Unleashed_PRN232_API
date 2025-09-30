using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Models;

namespace ProductService.Repositories.IRepositories
{
    public class ProductCategoryRepository : IProductCategoryRepository
    {
        private readonly ProductDbContext _context;

        public ProductCategoryRepository(ProductDbContext context)
        {
            _context = context;
        }

        public async Task AddCategoriesToProductAsync(Guid productId, List<int> categoryIds)
        {
            var productCategories = categoryIds.Select(categoryId => new ProductCategory
            {
                ProductId = productId,
                CategoryId = categoryId
            });

            _context.ProductCategories.AddRange(productCategories);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveCategoriesFromProductAsync(Guid productId, List<int> categoryIds)
        {
            var productCategories = await _context.ProductCategories
                .Where(pc => pc.ProductId == productId && categoryIds.Contains(pc.CategoryId.Value))
                .ToListAsync();

            _context.ProductCategories.RemoveRange(productCategories);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Category>> GetCategoriesByProductIdAsync(Guid productId)
        {
            return await _context.ProductCategories
                .Where(pc => pc.ProductId == productId)
                .Select(pc => pc.Category)
                .ToListAsync();
        }
    }
}
