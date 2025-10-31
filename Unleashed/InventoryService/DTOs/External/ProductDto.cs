namespace InventoryService.DTOs.External
{
    public class ProductDto
    {
        public Guid ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? BrandName { get; set; }
        public string? CategoryName { get; set; }
    }
}