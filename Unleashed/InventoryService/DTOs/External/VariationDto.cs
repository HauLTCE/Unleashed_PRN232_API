namespace InventoryService.DTOs.External
{
    public class VariationDto
    {
        public int VariationId { get; set; }
        public Guid? ProductId { get; set; }
        public int? SizeId { get; set; }
        public int? ColorId { get; set; }
        public string? VariationImage { get; set; }
        public decimal? VariationPrice { get; set; }
        public decimal? Price { get; set; }
        public SizeDto? Size { get; set; }
        public ColorDto? Color { get; set; }
        public ProductDto? Product { get; set; }
    }
}