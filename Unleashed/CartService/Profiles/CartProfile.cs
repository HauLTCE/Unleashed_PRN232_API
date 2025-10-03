using AutoMapper;
using CartService.Dtos;
using CartService.Models;

namespace CartService.Profiles
{
    public class CartProfile : Profile
    {
        public CartProfile() {
            CreateMap<Cart, CartDTO>();
            CreateMap<CreateCartDTO, Cart>();
            CreateMap<UpdateCartDTO, Cart>();

            CreateMap<CartDTO, Cart>();
            CreateMap<Cart, CreateCartDTO>();
            CreateMap<Cart,  UpdateCartDTO>();  
        }
    }
}
