using AutoMapper;
using ProductService.DTOs.ProductDTOs;
using ProductService.DTOs.VariationDTOs;
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

            CreateMap<CreateVariationDTO, Variation>();
            CreateMap<UpdateVariationDTO, Variation>();

            //model -> DTO
            CreateMap<Product, ProductDetailDTO>();
            CreateMap<Product, ProductListDTO>();
        }
    }
}
