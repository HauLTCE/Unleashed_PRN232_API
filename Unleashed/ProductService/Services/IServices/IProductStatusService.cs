using ProductService.Models;

namespace ProductService.Services.IServices
{
    public interface IProductStatusService
    {
        Task<IEnumerable<ProductStatus>> GetAllAsync();
        Task<ProductStatus?> GetByIdAsync(int id);
    }
}
