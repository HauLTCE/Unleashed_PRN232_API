using System.ComponentModel.DataAnnotations;

namespace InventoryService.DTOs.Internal
{
    public class StockTransactionDto
    {
        [Required]
        public int? ProviderId { get; set; }

        [Required]
        public int? StockId { get; set; }

        [Required]
        public string? Username { get; set; }

        [Required]
        public List<ProductVariationQuantityDto>? Variations { get; set; }
    }

    public class ProductVariationQuantityDto
    {
        [Required]
        public int? VariationId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int? Quantity { get; set; }
    }
}
