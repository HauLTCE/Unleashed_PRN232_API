using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Models;

[Table("transaction")]
public partial class Transaction
{
    [Key]
    [Column("transaction_id")]
    public int TransactionId { get; set; }

    [Column("stock_id")]
    public int? StockId { get; set; }

    [Column("variation_id")]
    public int? VariationId { get; set; }

    [Column("provider_id")]
    public int? ProviderId { get; set; }

    [Column("incharge_employee_id")]
    public Guid? InchargeEmployeeId { get; set; }

    [Column("transaction_type_id")]
    public int? TransactionTypeId { get; set; }

    [Column("transaction_quantity")]
    public int? TransactionQuantity { get; set; }

    [Column("transaction_date")]
    [Precision(6)]
    public DateTimeOffset? TransactionDate { get; set; }

    [Column("transaction_product_price", TypeName = "decimal(22, 2)")]
    public decimal? TransactionProductPrice { get; set; }

    [ForeignKey("ProviderId")]
    [InverseProperty("Transactions")]
    public virtual Provider? Provider { get; set; }

    [ForeignKey("StockId")]
    [InverseProperty("Transactions")]
    public virtual Stock? Stock { get; set; }

    [ForeignKey("TransactionTypeId")]
    [InverseProperty("Transactions")]
    public virtual TransactionType? TransactionType { get; set; }
}
