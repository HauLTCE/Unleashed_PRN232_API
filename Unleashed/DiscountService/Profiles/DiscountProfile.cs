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
            CreateMap<Discount, DiscountViewDto>()
                .ForMember(d => d.DiscountStatusName, opt => opt.MapFrom(s => s.DiscountStatus!.DiscountStatusName ?? string.Empty))
                .ForMember(d => d.DiscountTypeName, opt => opt.MapFrom(s => s.DiscountType!.DiscountTypeName ?? string.Empty));
        }
    }
}
