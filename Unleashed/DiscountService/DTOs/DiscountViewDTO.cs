// DTOs/DiscountViewDto.cs
namespace DiscountService.DTOs
{
    public class DiscountViewDto
    {
        public int DiscountId { get; set; }
        public string DiscountCode { get; set; }
        public decimal? DiscountValue { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public string? DiscountDescription { get; set; }
        public decimal? MinimumOrderValue { get; set; }
        public decimal? MaximumDiscountValue { get; set; }
        public int? UsageLimit { get; set; }
        public int? UsageCount { get; set; }

        // Thêm các trường tên để hiển thị
        public string? DiscountTypeName { get; set; }
        public string? DiscountStatusName { get; set; }
    }
}