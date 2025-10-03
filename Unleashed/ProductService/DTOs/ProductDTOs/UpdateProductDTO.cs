namespace ProductService.DTOs.ProductDTOs
{
    public class UpdateProductDTO
    {
        public Guid ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductDescription { get; set; }
        public int? BrandId { get; set; }
        public int? ProductStatusId { get; set; }
        public List<int>? CategoryIds { get; set; }
    }
}
