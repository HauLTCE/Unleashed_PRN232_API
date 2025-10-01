using InventoryService.DTOs.Provider;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryService.Services.Interfaces
{
    public interface IProviderService
    {
        Task<IEnumerable<ProviderDto>> GetAllProvidersAsync();
        Task<ProviderDto?> GetProviderByIdAsync(int id);
        Task<ProviderDto> CreateProviderAsync(CreateProviderDto providerDto);
        Task<bool> UpdateProviderAsync(int id, UpdateProviderDto providerDto);
        Task<bool> DeleteProviderAsync(int id);
    }
}