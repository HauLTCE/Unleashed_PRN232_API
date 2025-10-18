
using ProductService.Models;

namespace ProductService.Repositories.IRepositories
{
    public interface IColorRepository
    {
        Task<List<Color>> GetAllAsync();
        Task<List<Color>> GetAvailableAsync(bool onlyActiveProducts = false);
        Task<Color?> GetByIdAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
