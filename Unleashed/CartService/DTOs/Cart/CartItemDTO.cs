using CartService.DTOs.Client;

namespace CartService.DTOs.Cart
{
    public class CartItemDTO
    {
        public VariationDTO Variation { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public int StockQuantity { get; set; }
        public SaleDTO? Sale { get; set; }
        public decimal ? FinalPrice { get; set; }
    }
}
