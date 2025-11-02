using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Models;
using ProductService.Repositories.IRepositories;

namespace ProductService.Repositories
{
    public class ProductCategoryRepository : IProductCategoryRepository
    {
        private readonly ProductDbContext _context;
        private readonly ILogger<ProductCategoryRepository> _logger;

        public ProductCategoryRepository(ProductDbContext context, ILogger<ProductCategoryRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Thêm nhiều category cho 1 sản phẩm — chống trùng trước khi insert
        public async Task AddCategoriesToProductAsync(Guid productId, List<int> categoryIds)
        {
            if (categoryIds == null || categoryIds.Count == 0) return;

            // 1) Lấy các category đã tồn tại (dùng AsNoTracking + Raw SQL vẫn ổn)
            var existing = await _context.ProductCategories
                .AsNoTracking()
                .Where(pc => pc.ProductId == productId)
                .Select(pc => pc.CategoryId)
                .ToListAsync();

            // 2) Loại trùng
            var toAdd = categoryIds.Distinct().Except(existing).ToList();
            if (toAdd.Count == 0) return;

            // 3) Insert từng dòng bằng Raw SQL (an toàn, clear)
            foreach (var categoryId in toAdd)
            {
                try
                {
                    await _context.Database.ExecuteSqlInterpolatedAsync($@"
                        INSERT INTO product_category (product_id, category_id)
                        VALUES ({productId}, {categoryId});
                    ");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "AddCategory failed: productId={ProductId}, categoryId={CategoryId}", productId, categoryId);
                    throw;
                }
            }
        }

        // Xoá nhiều category khỏi 1 sản phẩm — Raw SQL
        public async Task RemoveCategoriesFromProductAsync(Guid productId, List<int> categoryIds)
        {
            if (categoryIds == null || categoryIds.Count == 0) return;

            foreach (var categoryId in categoryIds.Distinct())
            {
                try
                {
                    await _context.Database.ExecuteSqlInterpolatedAsync($@"
                        DELETE FROM product_category
                        WHERE product_id = {productId} AND category_id = {categoryId};
                    ");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "RemoveCategory failed: productId={ProductId}, categoryId={CategoryId}", productId, categoryId);
                    throw;
                }
            }
        }

        // Lấy danh sách Category của Product — vẫn có thể xài LINQ như cũ
        public async Task<List<Category>> GetCategoriesByProductIdAsync(Guid productId)
        {
            // Tuỳ DB schema: nếu ProductCategory không track được, vẫn join qua view keyless được
            return await _context.ProductCategories
                .AsNoTracking()
                .Where(pc => pc.ProductId == productId)
                .Select(pc => pc.Category!)
                .ToListAsync();
        }
        public async Task RemoveAllByProductAsync(Guid productId)
        {
            await _context.Database.ExecuteSqlInterpolatedAsync($@"
        DELETE FROM product_category WHERE product_id = {productId};
    ");
        }
    }
}
