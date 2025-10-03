
using ProductService.Models;

namespace ProductService.Repositories.IRepositories
{
    public interface IColorRepository
    {
        Task<Color?> GetByIdAsync(int id);
    }
}
