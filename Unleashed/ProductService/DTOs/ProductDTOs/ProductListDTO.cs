namespace ProductService.DTOs.ProductDTOs
{
    public class ProductListDTO
    {
        public Guid ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductCode { get; set; }
        public string? BrandName { get; set; }
        public string? ProductStatusName { get; set; }
        public string? VariationImage { get; set; }
        public decimal? VariationPrice { get; set; }
    }
}
