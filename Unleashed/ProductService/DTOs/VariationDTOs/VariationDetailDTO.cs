using ProductService.DTOs.OtherDTOs;

namespace ProductService.DTOs.VariationDTOs
{
    public class VariationDetailDTO
    {
        public int VariationId { get; set; }
        public Guid? ProductId { get; set; }
        public int? SizeId { get; set; }
        public int? ColorId { get; set; }
        public string? VariationImage { get; set; }
        public decimal? VariationPrice { get; set; }

        public SizeDTO? Size { get; set; }
        public ColorDTO? Color { get; set; }
    }
}
