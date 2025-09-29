using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OrderService.Models;

[PrimaryKey("OrderId", "VariationSingleId")]
[Table("order_variation_single")]
public partial class OrderVariationSingle
{
    [Key]
    [Column("order_id")]
    [StringLength(255)]
    public string OrderId { get; set; } = null!;

    [Key]
    [Column("variation_single_id")]
    public int VariationSingleId { get; set; }

    [Column("sale_id")]
    public int? SaleId { get; set; }

    [Column("variation_price_at_purchase", TypeName = "decimal(22, 2)")]
    public decimal VariationPriceAtPurchase { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("OrderVariationSingles")]
    public virtual Order Order { get; set; } = null!;
}
