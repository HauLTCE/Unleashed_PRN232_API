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
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<CreateVariationDTO, Variation>();
            CreateMap<UpdateVariationDTO, Variation>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<CreateCategoryDTO, Category>();
            CreateMap<CreateBrandDTO, Brand>();

            // --- Model -> DTO ---
            CreateMap<Product, ProductDetailDTO>();
            CreateMap<Product, ProductListDTO>();

            CreateMap<Variation, VariationDetailDTO>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.ProductName : null))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => (src.Product != null && src.Product.Brand != null) ? src.Product.Brand.BrandName : null))
                .ForMember(dest => dest.CategoryNames, opt => opt.MapFrom(src =>
                    (src.Product != null && src.Product.ProductCategories != null)
                    ? src.Product.ProductCategories.Select(pc => pc.Category.CategoryName)
                    : new List<string>()));

            CreateMap<ProductStatus, ProductStatusDTO>();
            CreateMap<Brand, BrandDetailDTO>();
            CreateMap<Category, CategoryDetailDTO>();
            CreateMap<Size, SizeDTO>();
            CreateMap<Color, ColorDTO>();
        }
    }
}
