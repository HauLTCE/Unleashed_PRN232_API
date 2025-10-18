namespace CartService.DTOs.Cart
{
    public class CartDTO
    {
        public Guid UserId { get; set; }
        public int VariationId { get; set; }
        public int? CartQuantity { get; set; }
    }
}