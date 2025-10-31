using ProductService.Models;

namespace ProductService.Services.IServices
{
    public interface ISizeService
    {
        Task<List<Size>> GetAllAsync();
        Task<List<Size>> GetAvailableAsync(bool onlyActiveProducts = false);
        Task<Size?> GetByIdAsync(int id);
    }
}
