using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Models;

[PrimaryKey("StockId", "VariationId")]
[Table("stock_variation")]
public partial class StockVariation
{
    [Key]
    [Column("variation_id")]
    public int VariationId { get; set; }

    [Key]
    [Column("stock_id")]
    public int StockId { get; set; }

    [Column("stock_quantity")]
    public int? StockQuantity { get; set; }

    [ForeignKey("StockId")]
    [InverseProperty("StockVariations")]
    public virtual Stock Stock { get; set; } = null!;
}
