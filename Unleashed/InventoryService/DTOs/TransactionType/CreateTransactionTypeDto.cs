using System.ComponentModel.DataAnnotations;

namespace InventoryService.DTOs.TransactionType
{
    public class CreateTransactionTypeDto
    {
        [Required]
        [StringLength(255)]
        public string? TransactionTypeName { get; set; }
    }
}