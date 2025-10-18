namespace CartService.DTOs
{
    public class CartItemDTO
    {
        public VariationDTO Variation { get; set; }
        public string ProductName { get; set; } // << Thêm trường này
        public int Quantity { get; set; }
        public int StockQuantity { get; set; }
        public object Sale { get; set; }
    }
}
