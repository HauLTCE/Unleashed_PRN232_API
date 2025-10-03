using System.ComponentModel.DataAnnotations;

namespace InventoryService.DTOs.Stock
{
    public class UpdateStockDto
    {
        [Required]
        [StringLength(255)]
        public string? StockName { get; set; }

        [StringLength(255)]
        public string? StockAddress { get; set; }
    }
}