using ProductService.Models;
using ProductService.Repositories.IRepositories;
using ProductService.Services.IServices;

namespace ProductService.Services
{
    public class ProductStatusService : IProductStatusService
    {
        private readonly IProductStatusRepository _repository;

        public ProductStatusService(IProductStatusRepository repository)
        {
            _repository = repository;
        }
        public async Task<IEnumerable<ProductStatus>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<ProductStatus?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }
    }
}
