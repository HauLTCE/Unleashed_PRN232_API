using ProductService.Models;

namespace ProductService.Services.IServices
{
    public interface IColorService
    {
        Task<List<Color>> GetAllAsync();
        Task<List<Color>> GetAvailableAsync(bool onlyActiveProducts = false);
        Task<Color?> GetByIdAsync(int id);
    }
}
