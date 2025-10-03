using ProductService.DTOs.Common;
using ProductService.Models;

namespace ProductService.Repositories.IRepositories
{
    public interface IProductRepository
    {
        Task<PagedResult<Product>> GetPagedAsync(PaginationParams pagination);
        Task<Product?> GetByIdAsync(Guid id);
        Task<Product> CreateAsync(Product product);
        Task<Product?> UpdateAsync(Product product);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);

        Task<bool> DeleteByProductIdAsync(Guid productId);
    }
}
