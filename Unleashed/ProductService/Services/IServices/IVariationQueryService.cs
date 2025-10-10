using ProductService.DTOs.VariationDTOs;

namespace ProductService.Services.IServices
{
    public interface IVariationQueryService
    {
        Task<VariationDetailDTO?> GetDetailByIdAsync(int id);
        Task<List<VariationDetailDTO>> GetDetailsByIdsAsync(IEnumerable<int> ids);
        Task<List<VariationDetailDTO>> SearchDetailsAsync(string? search, Guid? productId, int? colorId, int? sizeId);
    }
}
