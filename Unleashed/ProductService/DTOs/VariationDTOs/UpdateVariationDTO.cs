namespace ProductService.DTOs.VariationDTOs
{
    public class UpdateVariationDTO
    {
        public int VariationId { get; set; } = 0;
        public int? SizeId { get; set; }
        public int? ColorId { get; set; }
        public string? VariationImage { get; set; }
        public decimal? VariationPrice { get; set; }
    }
}
