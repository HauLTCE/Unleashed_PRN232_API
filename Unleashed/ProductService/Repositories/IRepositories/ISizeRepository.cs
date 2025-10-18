using ProductService.Models;

namespace ProductService.Repositories.IRepositories
{
    public interface ISizeRepository
    {
        Task<List<Size>> GetAllAsync();
        Task<List<Size>> GetAvailableAsync(bool onlyActiveProducts = false);
        Task<Size?> GetByIdAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
