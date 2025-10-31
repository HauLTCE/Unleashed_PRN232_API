namespace ReviewService.DTOs.External
{
    public class ProductSummaryDto
    {
        public Guid ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductImageUrl { get; set; }
    }
}