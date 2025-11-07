using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace OrderService.Models;

[PrimaryKey("OrderId", "VariationId")]
[Table("order_variation")]
public partial class OrderVariation
{
    [Key]
    [Column("order_id")]
    [StringLength(255)]
    public Guid OrderId { get; set; }

    [Key]
    [Column("variation_id")]
    public int VariationId { get; set; }

    [Column("Quantity")]
    public int? Quantity { get; set; }

    [Column("sale_id")]
    public int? SaleId { get; set; }

    [Column("variation_price_at_purchase", TypeName = "decimal(22, 2)")]
    public decimal VariationPriceAtPurchase { get; set; }


    [JsonIgnore]
    [ForeignKey("OrderId")]
    [InverseProperty("OrderVariations")]
    public virtual Order Order { get; set; } = null!;
}
