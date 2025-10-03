using System.ComponentModel.DataAnnotations;

namespace InventoryService.DTOs.Transaction
{
    public class CreateTransactionDto
    {
        [Required]
        public int? StockId { get; set; }

        [Required]
        public int? VariationId { get; set; }

        public int? ProviderId { get; set; }

        public Guid? InchargeEmployeeId { get; set; }

        [Required]
        public int? TransactionTypeId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Transaction quantity must be greater than 0.")]
        public int? TransactionQuantity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? TransactionProductPrice { get; set; }
    }
}