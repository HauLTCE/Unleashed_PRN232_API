using ProductService.Models;

namespace ProductService.Repositories.IRepositories
{
    public interface IProductCategoryRepository
    {
        Task AddCategoriesToProductAsync(Guid productId, List<int> categoryIds);
        Task RemoveCategoriesFromProductAsync(Guid productId, List<int> categoryIds);
        Task<List<Category>> GetCategoriesByProductIdAsync(Guid productId);
        Task RemoveAllByProductAsync(Guid productId);
    }
}
