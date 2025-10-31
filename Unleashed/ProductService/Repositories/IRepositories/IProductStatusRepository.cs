using ProductService.Models;

namespace ProductService.Repositories.IRepositories
{
    public interface IProductStatusRepository
    {
        Task<IEnumerable<ProductStatus>> GetAllAsync();
        Task<ProductStatus?> GetByIdAsync(int id);
    }
}
