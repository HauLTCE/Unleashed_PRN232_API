using AutoMapper;
using InventoryService.DTOs.StockVariation;
using InventoryService.Models;

namespace InventoryService.Profiles
{
    public class StockVariationProfile : Profile
    {
        public StockVariationProfile()
        {
            // Source -> Target
            CreateMap<StockVariation, StockVariationDto>();
            CreateMap<CreateStockVariationDto, StockVariation>();
            CreateMap<UpdateStockVariationDto, StockVariation>();
        }
    }
}