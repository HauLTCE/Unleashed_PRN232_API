using ProductService.DTOs.Common;
using ProductService.Models;

namespace ProductService.Repositories.IRepositories
{
    public interface IBrandRepository
    {
        Task<PagedResult<Brand>> GetPagedAsync(PaginationParams pagination);
        Task<Brand?> GetByIdAsync(int id);
        Task<Brand> CreateAsync(Brand brand);
        Task<Brand?> UpdateAsync(Brand brand);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
