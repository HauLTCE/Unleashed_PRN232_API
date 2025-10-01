using System.ComponentModel.DataAnnotations;

namespace InventoryService.DTOs.StockVariation
{
    public class CreateStockVariationDto
    {
        [Required]
        public int VariationId { get; set; }

        [Required]
        public int StockId { get; set; }

        [Range(0, int.MaxValue)]
        public int? StockQuantity { get; set; }
    }
}