using AutoMapper;
using OrderService.Dtos;
using OrderService.Models;

namespace OrderService.Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile() 
        {
            CreateMap<OrderVariationSingle, OrderVariationSingleDto>();
            CreateMap<CreateOrderVariationSingleDto, OrderVariationSingle>();

            // Order Mappings
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.OrderStatusName, opt => opt.MapFrom(src => src.OrderStatus.OrderStatusName))
                .ForMember(dest => dest.PaymentMethodName, opt => opt.MapFrom(src => src.PaymentMethod.PaymentMethodName))
                .ForMember(dest => dest.ShippingMethodName, opt => opt.MapFrom(src => src.ShippingMethod.ShippingMethodName));

            CreateMap<CreateOrderDto, Order>();
            CreateMap<UpdateOrderDto, Order>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
