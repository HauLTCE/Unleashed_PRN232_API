using InventoryService.DTOs.External;

namespace InventoryService.Clients.Interfaces
{
    public interface IProductCatalogClient
    {
        Task<VariationDto?> GetVariationByIdAsync(int variationId);
        Task<IEnumerable<VariationDto>> GetVariationsByIdsAsync(IEnumerable<int> variationIds);
    }
}
