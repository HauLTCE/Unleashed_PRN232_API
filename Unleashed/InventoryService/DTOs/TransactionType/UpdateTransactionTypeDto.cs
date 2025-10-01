using System.ComponentModel.DataAnnotations;

namespace InventoryService.DTOs.TransactionType
{
    public class UpdateTransactionTypeDto
    {
        [Required]
        [StringLength(255)]
        public string? TransactionTypeName { get; set; }
    }
}