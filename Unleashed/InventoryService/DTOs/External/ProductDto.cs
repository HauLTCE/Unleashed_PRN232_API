namespace InventoryService.DTOs.External
{
    public class ProductDto
    {
        public string? ProductName { get; set; }
        public string? BrandName { get; set; }
        public List<string>? CategoryNames { get; set; }
    }
}