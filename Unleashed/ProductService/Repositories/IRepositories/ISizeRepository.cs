using ProductService.Models;

namespace ProductService.Repositories.IRepositories
{
    public interface ISizeRepository
    {
        Task<Size?> GetByIdAsync(int id);
    }
}
