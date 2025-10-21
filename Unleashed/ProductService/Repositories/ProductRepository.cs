using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.DTOs.CategoryDTOs;
using ProductService.DTOs.Common;
using ProductService.DTOs.ProductDTOs;
using ProductService.DTOs.SaleDTOs;
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

        public async Task<PagedResult<ProductListDTO>> GetPagedForProductListAsync(PaginationParams pagination)
        {
            var now = DateTimeOffset.UtcNow;

            // Base query + filter
            var baseQuery = _context.Products.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(pagination.Search))
            {
                var search = pagination.Search.Trim();
                baseQuery = baseQuery.Where(p =>
                    (p.ProductName ?? "").Contains(search) ||
                    (p.ProductCode ?? "").Contains(search));
            }

            baseQuery = baseQuery.OrderByDescending(p => p.ProductCreatedAt);

            // Project thẳng ra ProductListDTO
            var projected = baseQuery.Select(p => new ProductListDTO
            {
                // Core
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                ProductCode = p.ProductCode,
                ProductDescription = p.ProductDescription,

                // Brand
                BrandId = p.BrandId,
                BrandName = _context.Brands
                    .Where(b => b.BrandId == p.BrandId)
                    .Select(b => b.BrandName)
                    .FirstOrDefault(),

                // Status
                ProductStatusName = _context.ProductStatuses
                    .Where(s => s.ProductStatusId == p.ProductStatusId)
                    .Select(s => s.ProductStatusName)
                    .FirstOrDefault(),

                // Categories (giả định có ProductCategories & Categories)
                CategoryList =
                    (from pc in _context.ProductCategories
                     join c in _context.Categories on pc.CategoryId equals c.CategoryId
                     where pc.ProductId == p.ProductId
                     orderby c.CategoryName
                     select new CategoryDTO
                     {
                         CategoryId = c.CategoryId,
                         CategoryName = c.CategoryName,
                         // thêm field khác nếu DTO có
                     }).ToList(),

                // Variation hiển thị (lấy variation đầu tiên — Sếp có thể đổi sort rule)
                VariationImage = _context.Variations
                    .Where(v => v.ProductId == p.ProductId)
                    .OrderBy(v => v.VariationId)
                    .Select(v => v.VariationImage)           // đổi theo tên thật trong Model
                    .FirstOrDefault(),

                VariationPrice = _context.Variations
                    .Where(v => v.ProductId == p.ProductId)
                    .OrderBy(v => v.VariationId)
                    .Select(v => (decimal?)v.VariationPrice)    // đổi theo tên thật trong Model
                    .FirstOrDefault(),

                // Giá gốc (nếu không có cột Price ở Product, lấy min giá biến thể)
                ProductPrice = _context.Variations
                    .Where(v => v.ProductId == p.ProductId)
                    .Select(v => (decimal?)v.VariationPrice)    // đổi theo tên thật trong Model
                    .Min(),

                // CTKM đang hiệu lực (đưa vào SaleDTO)
                Sale = (
                    from sp in _context.SaleProducts
                    join s in _context.Sales on sp.SaleId equals s.SaleId
                    where sp.ProductId == p.ProductId
                          && (s.SaleStartDate == null || s.SaleStartDate <= now)
                          && (s.SaleEndDate == null || s.SaleEndDate >= now)
                    orderby s.SaleValue descending, s.SaleStartDate descending
                    select new SaleDTO
                    {
                        SaleId = s.SaleId,
                        SaleTypeId = s.SaleTypeId,
                        SaleStatusId = s.SaleStatusId,
                        SaleValue = s.SaleValue,
                        SaleStartDate = s.SaleStartDate,
                        SaleEndDate = s.SaleEndDate
                    }
                ).FirstOrDefault(),

                // Giá trị KM (flatten cho FE dễ dùng – tách riêng)
                SaleValue = (
                    from sp in _context.SaleProducts
                    join s in _context.Sales on sp.SaleId equals s.SaleId
                    where sp.ProductId == p.ProductId
                          && (s.SaleStartDate == null || s.SaleStartDate <= now)
                          && (s.SaleEndDate == null || s.SaleEndDate >= now)
                    orderby s.SaleValue descending, s.SaleStartDate descending
                    select s.SaleValue
                ).FirstOrDefault(),

                // Ratings
                AverageRating = _context.Reviews
                    .Where(r => r.ProductId == p.ProductId && r.ReviewRating != null)
                    .Select(r => (double)r.ReviewRating!)
                    .DefaultIfEmpty(0)
                    .Average(),

                TotalRatings = _context.Reviews
                    .LongCount(r => r.ProductId == p.ProductId),

                // Inventory (tổng tồn kho)
                Quantity =
                    (from v in _context.Variations
                     join sv in _context.StockVariations on v.VariationId equals sv.VariationId
                     where v.ProductId == p.ProductId
                     select (int?)sv.StockQuantity).Sum() ?? 0
            });

            // Trả về PagedResult<ProductListDTO>
            return await projected.ToPagedResultAsync(pagination.PageNumber, pagination.PageSize);
        }
        public async Task<PagedResult<Product>> GetPagedAsync(PaginationParams pagination)
        {
            var query = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.ProductStatus)
                .Include(p => p.Variations)
                .AsQueryable();

            // Search
            if (!string.IsNullOrEmpty(pagination.Search))
            {
                query = query.Where(p =>
                    p.ProductName.Contains(pagination.Search) ||
                    p.ProductCode.Contains(pagination.Search));
            }

            // Order by latest
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

    }
}
