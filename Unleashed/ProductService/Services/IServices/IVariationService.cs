using ProductService.DTOs.Common;
using ProductService.DTOs.VariationDTOs;

namespace ProductService.Services.IServices
{
    public interface IVariationService
    {
        Task<PagedResult<VariationDetailDTO>> GetPagedVariationsByProductIdAsync(Guid productId, PaginationParams pagination);
        Task<VariationDetailDTO?> GetVariationByIdAsync(int id);
        Task<VariationDetailDTO> CreateVariationAsync(CreateVariationDTO createVariationDto, Guid productId);
        Task<VariationDetailDTO?> UpdateVariationAsync(int id, UpdateVariationDTO updateVariationDto);
        Task<bool> DeleteVariationAsync(int id);
    }
}
