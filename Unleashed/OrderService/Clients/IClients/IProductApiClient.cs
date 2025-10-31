using OrderService.DTOs.ResponesDtos;

namespace ProductService.Clients.IClients
{
    public interface IProductApiClient
    {
       
        Task<IEnumerable<VariationResponseDto>> GetDetailsByIdsAsync(IEnumerable<int> variationIds);
    }
}
