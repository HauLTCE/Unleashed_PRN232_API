using AutoMapper;
using ProductService.DTOs.BrandDTOs;
using ProductService.DTOs.CategoryDTOs;
using ProductService.DTOs.OtherDTOs;
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
            CreateMap<ProductStatus, ProductStatusDTO>();
            CreateMap<Brand, BrandDetailDTO>();
            CreateMap<Category, CategoryDetailDTO>();
            CreateMap<Size, SizeDTO>();
            CreateMap<Color, ColorDTO>();
            CreateMap<CreateCategoryDTO, Category>();
            CreateMap<CreateBrandDTO, Brand>();

            //model -> DTO
            CreateMap<Product, ProductDetailDTO>();
            CreateMap<Product, ProductListDTO>();
            CreateMap<Brand, BrandDetailDTO>();
            CreateMap<Category, CategoryDetailDTO>();
            CreateMap<Variation, VariationDetailDTO>();
            CreateMap<Size, SizeDTO>();
            CreateMap<Color, ColorDTO>();
            CreateMap<ProductStatus, ProductStatusDTO>();
            CreateMap<Brand, BrandDetailDTO>();
            CreateMap<Category, CategoryDetailDTO>();
        }
    }
}
