namespace InventoryService.DTOs.StockVariation
{
    public class StockVariationDto
    {
        public int VariationId { get; set; }
        public int StockId { get; set; }
        public int? StockQuantity { get; set; }
    }
}