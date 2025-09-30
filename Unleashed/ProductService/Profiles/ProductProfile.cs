using AutoMapper;
using ProductService.DTOs.ProductDTOs;
using ProductService.Models;

namespace ProductService.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            //map DTO -> Model
            CreateMap<CreateProductDTO, Product>();
            CreateMap<UpdateProductDTO, Product>();

            //model -> DTO
            CreateMap<Product, ProductDetailDTO>();
            CreateMap<Product, ProductListDTO>();
        }
    }
}
