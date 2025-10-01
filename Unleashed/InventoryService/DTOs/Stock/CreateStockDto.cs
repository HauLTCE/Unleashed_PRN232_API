using System.ComponentModel.DataAnnotations;

namespace InventoryService.DTOs.Stock
{
    public class CreateStockDto
    {
        [Required]
        [StringLength(255)]
        public string? StockName { get; set; }

        [StringLength(255)]
        public string? StockAddress { get; set; }
    }
}