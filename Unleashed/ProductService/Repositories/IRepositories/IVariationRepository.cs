using ProductService.DTOs.Common;
using ProductService.Models;

namespace ProductService.Repositories.IRepositories
{
    public interface IVariationRepository
    {
        Task<PagedResult<Variation>> GetPagedByProductIdAsync(Guid productId, PaginationParams pagination);
        Task<Variation?> GetByIdAsync(int id);
        Task<Variation> CreateAsync(Variation variation);
        Task<Variation?> UpdateAsync(Variation variation);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
