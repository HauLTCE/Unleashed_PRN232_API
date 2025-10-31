using ProductService.Models;
using ProductService.Repositories.IRepositories;
using ProductService.Services.IServices;

namespace ProductService.Services
{
    public class ColorService : IColorService
    {
        private readonly IColorRepository _colorRepository;

        public ColorService(IColorRepository colorRepository)
        {
            _colorRepository = colorRepository;
        }

        public async Task<List<Color>> GetAllAsync()
        {
            return await _colorRepository.GetAllAsync();
        }

        public async Task<List<Color>> GetAvailableAsync(bool onlyActiveProducts = false)
        {
            return await _colorRepository.GetAvailableAsync(onlyActiveProducts);
        }

        public async Task<Color?> GetByIdAsync(int id)
        {
            return await _colorRepository.GetByIdAsync(id);
        }
    }
}
