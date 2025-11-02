namespace InventoryService.DTOs.External
{
    public class FlatVariationDto
    {
        public int VariationId { get; set; }
        public Guid? ProductId { get; set; }
        public int? SizeId { get; set; }
        public int? ColorId { get; set; }
        public string? VariationImage { get; set; }
        public decimal? VariationPrice { get; set; }
        public string? ProductName { get; set; }
        public string? BrandName { get; set; }
        public List<string>? CategoryNames { get; set; }
        public SizeDto? Size { get; set; }
        public ColorDto? Color { get; set; }
    }
}