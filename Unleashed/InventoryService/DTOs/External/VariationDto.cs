namespace InventoryService.DTOs.External
{
    public class VariationDto
    {
        public int Id { get; set; }
        public Guid ProductId { get; set; }
        public decimal Price { get; set; }
        public string? ProductName { get; set; }
        public string? ColorName { get; set; }
        public string? SizeName { get; set; }
    }
}
