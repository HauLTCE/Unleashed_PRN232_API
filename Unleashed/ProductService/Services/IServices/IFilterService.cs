using ProductService.DTOs.OtherDTOs;

namespace ProductService.Services.IServices
{
    public interface IFilterService
    {
        Task<FilterOptionsDTO> GetFilterOptionsAsync(bool onlyAvailable = false, bool onlyActiveProducts = false);
    }
}
