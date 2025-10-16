namespace CartService.DTOs
{
    public class CartItemDTO
    {
        public VariationDTO Variation { get; set; }
        public int Quantity { get; set; }
        public int StockQuantity { get; set; }
    }
}
