namespace CartService.DTOs
{
    public class GroupedCartDTO
    {
        public string ProductName { get; set; }
        public List<CartItemDTO> Items { get; set; }
    }
}
