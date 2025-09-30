using ProductService.DTOs.BrandDTOs;
using ProductService.DTOs.CategoryDTOs;
using ProductService.DTOs.OtherDTOs;
using ProductService.DTOs.VariationDTOs;

namespace ProductService.DTOs.ProductDTOs
{
    public class ProductDetailDTO
    {
        public Guid ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductDescription { get; set; }
        public int? BrandId { get; set; }
        public int? ProductStatusId { get; set; }
        public DateTimeOffset? ProductCreatedAt { get; set; }
        public DateTimeOffset? ProductUpdatedAt { get; set; }

        public BrandDetailDTO? Brand { get; set; }
        public ProductStatusDTO? ProductStatus { get; set; }
        public List<CategoryDetailDTO>? Categories { get; set; }
        public List<VariationDetailDTO>? Variations { get; set; }
    }
}
