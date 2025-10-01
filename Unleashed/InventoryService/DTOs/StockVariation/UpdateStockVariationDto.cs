using System.ComponentModel.DataAnnotations;

namespace InventoryService.DTOs.StockVariation
{
    public class UpdateStockVariationDto
    {
        [Required]
        [Range(0, int.MaxValue)]
        public int? StockQuantity { get; set; }
    }
}