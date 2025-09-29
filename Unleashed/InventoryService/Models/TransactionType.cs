using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Models;

[Table("transaction_type")]
public partial class TransactionType
{
    [Key]
    [Column("transaction_type_id")]
    public int TransactionTypeId { get; set; }

    [Column("transaction_type_name")]
    [StringLength(255)]
    public string? TransactionTypeName { get; set; }

    [InverseProperty("TransactionType")]
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
