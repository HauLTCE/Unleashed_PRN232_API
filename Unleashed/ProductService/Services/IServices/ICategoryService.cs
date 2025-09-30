using ProductService.DTOs.CategoryDTOs;
using ProductService.DTOs.Common;

namespace ProductService.Services.IServices
{
    public interface ICategoryService
    {
        Task<PagedResult<CategoryDetailDTO>> GetPagedCategoriesAsync(PaginationParams pagination);
        Task<CategoryDetailDTO?> GetCategoryByIdAsync(int id);
        Task<CategoryDetailDTO> CreateCategoryAsync(CreateCategoryDTO createCategoryDto);
        Task<CategoryDetailDTO?> UpdateCategoryAsync(int id, UpdateCategoryDTO updateCategoryDto);
        Task<bool> DeleteCategoryAsync(int id);
    }
}
