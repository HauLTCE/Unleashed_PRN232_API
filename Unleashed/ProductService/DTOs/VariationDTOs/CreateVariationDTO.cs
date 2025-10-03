namespace ProductService.DTOs.VariationDTOs
{
    public class CreateVariationDTO
    {
        public int? SizeId { get; set; }
        public int? ColorId { get; set; }
        public string? VariationImage { get; set; }
        public decimal? VariationPrice { get; set; }
    }
}
