using ProductService.Models;
using ProductService.Repositories.IRepositories;
using ProductService.Services.IServices;

namespace ProductService.Services
{
    public class SizeService : ISizeService
    {
        private readonly ISizeRepository _sizeRepository;

        public SizeService(ISizeRepository sizeRepository)
        {
            _sizeRepository = sizeRepository;
        }

        public async Task<List<Size>> GetAllAsync()
        {
            return await _sizeRepository.GetAllAsync();
        }

        public async Task<List<Size>> GetAvailableAsync(bool onlyActiveProducts = false)
        {
            return await _sizeRepository.GetAvailableAsync(onlyActiveProducts);
        }

        public async Task<Size?> GetByIdAsync(int id)
        {
            return await _sizeRepository.GetByIdAsync(id);
        }
    }
}
