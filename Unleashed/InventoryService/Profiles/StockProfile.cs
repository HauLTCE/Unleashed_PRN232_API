using AutoMapper;
using InventoryService.DTOs.Stock;
using InventoryService.Models;

namespace InventoryService.Profiles
{
    public class StockProfile : Profile
    {
        public StockProfile()
        {
            // Source -> Target
            CreateMap<Stock, StockDto>();
            CreateMap<CreateStockDto, Stock>();
            CreateMap<UpdateStockDto, Stock>();
        }
    }
}