using ProductService.DTOs.CategoryDTOs;
using ProductService.DTOs.SaleDTOs;

namespace ProductService.DTOs.ProductDTOs
{
    public class ProductListDTO
    {
        public Guid ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductDescription { get; set; }

        // Brand
        public int? BrandId { get; set; }
        public string? BrandName { get; set; }

        // Status
        public string? ProductStatusName { get; set; }

        // Categories
        public List<CategoryDTO> CategoryList { get; set; } = new();

        // Variation display
        public string? VariationImage { get; set; }
        public decimal? VariationPrice { get; set; }  // Giá của biến thể hiển thị

        // Pricing
        public decimal? ProductPrice { get; set; }    // Giá gốc (nếu có)
        public SaleDTO? Sale { get; set; }            // Thông tin CTKM (tùy chọn)
        public decimal? SaleValue { get; set; }       // Giá trị KM (tỷ lệ % hoặc số tiền)

        // Ratings
        public double? AverageRating { get; set; }
        public long? TotalRatings { get; set; }

        // Inventory
        public int Quantity { get; set; }
    }
}
