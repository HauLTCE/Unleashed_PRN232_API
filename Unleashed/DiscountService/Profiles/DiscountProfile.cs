using AutoMapper;
using DiscountService.DTOs;
using DiscountService.Models;

namespace DiscountService.Profiles
{
    public class DiscountProfile : Profile
    {
        public DiscountProfile()
        {
            // Source -> Target
            CreateMap<Discount, DiscountDto>();
            CreateMap<CreateDiscountDto,Discount>();
            CreateMap<UpdateDiscountDto, Discount>();
            CreateMap<DiscountViewDto, Discount>();
            CreateMap<DiscountDto,Discount>();
            CreateMap<Discount,CreateDiscountDto>();
            CreateMap<Discount, UpdateDiscountDto>();
            CreateMap<Discount, DiscountViewDto>();
        }
    }
}
