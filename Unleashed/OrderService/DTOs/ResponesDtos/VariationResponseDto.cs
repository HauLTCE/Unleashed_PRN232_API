namespace OrderService.DTOs.ResponesDtos
{
    public class VariationResponseDto
    {
        public int VariationId { get; set; }
        public Guid ProductId { get; set; }
        public double VariationPrice { get; set; }
    }
}
