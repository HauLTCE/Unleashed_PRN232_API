using ProductService.DTOs.Common;
using ProductService.Models;

namespace ProductService.Repositories.IRepositories
{
    public interface ICategoryRepository
    {
        Task<PagedResult<Category>> GetPagedAsync(PaginationParams pagination);
        Task<Category?> GetByIdAsync(int id);
        Task<Category> CreateAsync(Category category);
        Task<Category?> UpdateAsync(Category category);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
