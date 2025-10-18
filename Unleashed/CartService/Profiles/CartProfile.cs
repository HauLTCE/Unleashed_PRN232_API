using AutoMapper;
using CartService.DTOs.Cart;
using CartService.DTOs.Client;
using CartService.Models;

namespace CartService.Profiles
{
    public class CartProfile : Profile
    {
        public CartProfile() {
            CreateMap<Cart, CartDTO>();
            CreateMap<CreateCartDTO, Cart>();
            CreateMap<UpdateCartDTO, Cart>();
            CreateMap<CartItemDTO, Cart>();
            CreateMap<GroupedCartDTO, Cart>();
            CreateMap<VariationDTO, Cart>();

            CreateMap<CartDTO, Cart>();
            CreateMap<Cart, CreateCartDTO>();
            CreateMap<Cart,  UpdateCartDTO>();
            CreateMap<Cart, CartItemDTO>();
            CreateMap<Cart, GroupedCartDTO>();
            CreateMap<Cart, VariationDTO>();
        }
    }
}
