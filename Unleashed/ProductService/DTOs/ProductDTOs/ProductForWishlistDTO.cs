namespace ProductService.DTOs.ProductDTOs
{
    public class ProductForWishlistDTO
    {
        public Guid ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductImage { get; set; }
        public int? ProductStatusId { get; set; }
    }
}