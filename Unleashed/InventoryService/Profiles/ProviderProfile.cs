using AutoMapper;
using InventoryService.DTOs.Provider;
using InventoryService.Models;

namespace InventoryService.Profiles
{
    public class ProviderProfile : Profile
    {
        public ProviderProfile()
        {
            // Source -> Target
            // For GET requests (Entity to DTO)
            CreateMap<Provider, ProviderDto>();

            // For POST requests (DTO to Entity)
            CreateMap<CreateProviderDto, Provider>();

            // For PUT requests (DTO to Entity)
            CreateMap<UpdateProviderDto, Provider>();
        }
    }
}