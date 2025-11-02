namespace ReviewService.DTOs.External
{
    public class OrderDto
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public int? OrderStatus { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = new();
    }

    public class OrderItemDto
    {
        public Guid ProductId { get; set; }
    }
}
