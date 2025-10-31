namespace ProductService.DTOs.ProductDTOs
{
    public class ProductSummaryDTO
    {
        public Guid ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductImageUrl { get; set; }
    }
}