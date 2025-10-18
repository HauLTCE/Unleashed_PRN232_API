namespace CartService.DTOs.Client
{
    public class WishlistProductInfoDTO
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public int ProductStatusId { get; set; }
    }
}