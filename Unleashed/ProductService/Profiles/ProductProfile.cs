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
            // --- DTO -> Model ---
            CreateMap<CreateProductDTO, Product>();
            CreateMap<UpdateProductDTO, Product>()
                // BỎ QUA CÁC THUỘC TÍNH NULL TRONG DTO KHI MAP SANG ENTITY
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<CreateVariationDTO, Variation>();
            CreateMap<UpdateVariationDTO, Variation>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<CreateCategoryDTO, Category>();
            CreateMap<CreateBrandDTO, Brand>();

            // --- Model -> DTO ---
            CreateMap<Product, ProductDetailDTO>();
            CreateMap<Product, ProductListDTO>();
            CreateMap<Variation, VariationDetailDTO>();
            CreateMap<ProductStatus, ProductStatusDTO>();
            CreateMap<Brand, BrandDetailDTO>();
            CreateMap<Category, CategoryDetailDTO>();
            CreateMap<Size, SizeDTO>();
            CreateMap<Color, ColorDTO>();
        }
    }
}
