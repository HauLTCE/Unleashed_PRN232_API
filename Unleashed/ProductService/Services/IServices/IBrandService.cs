using ProductService.DTOs.BrandDTOs;
using ProductService.DTOs.Common;

namespace ProductService.Services.IServices
{
    public interface IBrandService
    {
        Task<PagedResult<BrandDetailDTO>> GetPagedBrandsAsync(PaginationParams pagination);
        Task<BrandDetailDTO?> GetBrandByIdAsync(int id);
        Task<BrandDetailDTO> CreateBrandAsync(CreateBrandDTO createBrandDto);
        Task<BrandDetailDTO?> UpdateBrandAsync(int id, UpdateBrandDTO updateBrandDto);
        Task<bool> DeleteBrandAsync(int id);
    }
}
