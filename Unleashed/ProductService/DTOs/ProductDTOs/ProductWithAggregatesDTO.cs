using ProductService.DTOs.OtherDTOs;
using ProductService.DTOs.SaleDTOs;

namespace ProductService.DTOs.ProductDTOs
{
    public class ProductWithAggregatesDTO
    {
        // Product core
        public Guid ProductId { get; set; }
        public int? BrandId { get; set; }
        public int? ProductStatusId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductDescription { get; set; }
        public DateTimeOffset? ProductCreatedAt { get; set; }
        public DateTimeOffset? ProductUpdatedAt { get; set; }

        // Aggregates
        public double AverageRating { get; set; }
        public int RatingsCount { get; set; }
        public int TotalStockQuantity { get; set; }

        // Active sale (null nếu không có)
        public SaleDTO? ActiveSale { get; set; }

        // (Tuỳ chọn) thông tin variation đầu tiên
        public int? FirstVariationId { get; set; }
    }
}
