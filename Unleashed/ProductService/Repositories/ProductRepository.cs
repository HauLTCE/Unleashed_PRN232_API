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
            var query = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.ProductStatus)
                .Include(p => p.Variations)
                    .ThenInclude(v => v.Color)
                .Include(p => p.Variations)
                    .ThenInclude(v => v.Size)
                .Join(_context.ProductCategories,
                    product => product.ProductId,
                    productCategory => productCategory.ProductId,
                    (product, productCategory) => new { product, productCategory })
                .Join(_context.Categories,
                    combined => combined.productCategory.CategoryId,
                    category => category.CategoryId,
                    (combined, category) => new { combined.product, category })
                .Join(_context.SaleProducts,
                    combined => combined.product.ProductId,
                    saleProduct => saleProduct.ProductId,
                    (combined, saleProduct) => new { combined.product, combined.category, saleProduct })
                .Join(_context.Sales,
                    combined => combined.saleProduct.SaleId,
                    sale => sale.SaleId,
                    (combined, sale) => new { combined.product, combined.category, sale, combined.saleProduct })
                .Join(_context.Reviews,
                    combined => combined.product.ProductId,
                    review => review.ProductId,
                    (combined, review) => new { combined.product, combined.category, combined.sale, combined.saleProduct, review })
                .Join(_context.StockVariations,
                    combined => combined.product.Variations.FirstOrDefault().VariationId, // Join theo VariationId
                    stock => stock.VariationId,
                    (combined, stock) => new { combined.product, combined.category, combined.sale, combined.saleProduct, combined.review, stock })
                .AsQueryable();

            // Áp dụng search nếu có
            if (!string.IsNullOrEmpty(pagination.Search))
            {
                query = query.Where(p => p.product.ProductName.Contains(pagination.Search));
            }

            // Lấy tổng số sản phẩm
            var totalCount = await query.CountAsync();

            // Tính toán Tổng số đánh giá và Đánh giá trung bình cho mỗi sản phẩm
            var items = await query
                .GroupBy(p => p.product.ProductId)  // Group by ProductId để tính tổng và trung bình
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .Select(p => new ProductListDTO
                {
                    ProductId = p.Key,
                    ProductName = p.FirstOrDefault().product.ProductName,
                    ProductCode = p.FirstOrDefault().product.ProductCode,
                    ProductDescription = p.FirstOrDefault().product.ProductDescription,
                    BrandId = p.FirstOrDefault().product.BrandId,
                    BrandName = p.FirstOrDefault().product.Brand.BrandName,
                    ProductStatusName = p.FirstOrDefault().product.ProductStatus.ProductStatusName,
                    CategoryList = new List<CategoryDTO>
                    {
                new CategoryDTO
                {
                    CategoryId = p.FirstOrDefault().category.CategoryId,
                    CategoryName = p.FirstOrDefault().category.CategoryName
                }
                    },
                    VariationImage = p.FirstOrDefault().product.Variations.FirstOrDefault().VariationImage,
                    VariationPrice = p.FirstOrDefault().product.Variations.FirstOrDefault().VariationPrice,
                    ProductPrice = p.FirstOrDefault().product.Variations.FirstOrDefault().VariationPrice,
                    Sale = p.FirstOrDefault().sale != null ? new SaleDTO
                    {
                        SaleId = p.FirstOrDefault().sale.SaleId,
                        SaleTypeId = p.FirstOrDefault().sale.SaleTypeId,
                        SaleStatusId = p.FirstOrDefault().sale.SaleStatusId,
                        SaleValue = p.FirstOrDefault().sale.SaleValue,
                        SaleStartDate = p.FirstOrDefault().sale.SaleStartDate,
                        SaleEndDate = p.FirstOrDefault().sale.SaleEndDate
                    } : null,
                    SaleValue = p.FirstOrDefault().sale.SaleValue ?? 0,
                    AverageRating = p.Average(r => r.review.ReviewRating) ?? 0,  // Tính trung bình đánh giá
                    TotalRatings = p.Count(),  // Tính tổng số đánh giá
                    Quantity = p.FirstOrDefault().stock != null ? p.FirstOrDefault().stock.StockQuantity ?? 0 : 0 // Tính tổng số lượng từ StockVariation
                }).ToListAsync();

            return new PagedResult<ProductListDTO>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };
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
