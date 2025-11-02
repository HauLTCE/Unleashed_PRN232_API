namespace OrderService.DTOs.ResponesDtos
{
    public class VariationResponseDto
    {
        public int VariationId { get; set; }
        public Guid ProductId { get; set; }
        public decimal VariationPrice { get; set; }
    }
}
