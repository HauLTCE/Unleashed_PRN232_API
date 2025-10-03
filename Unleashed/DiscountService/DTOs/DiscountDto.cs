namespace DiscountService.DTOs
{
    public class DiscountDto
    {
        public int DiscountId { get; set; }
        public int? DiscountStatusId { get; set; }
        public int? DiscountTypeId { get; set; }
        public string DiscountCode { get; set; }
        public decimal? DiscountValue { get; set; }
        public string? DiscountDescription { get; set; }
        public decimal? DiscountMinimumOrderValue { get; set; }
        public decimal? DiscountMaximumValue { get; set; }
        public int? DiscountUsageLimit { get; set; }
        public DateTimeOffset? DiscountStartDate { get; set; }
        public DateTimeOffset? DiscountEndDate { get; set; }
        public int? DiscountUsageCount { get; set; }
    }
}
