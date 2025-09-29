using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DiscountService.Models;

[PrimaryKey("SaleId", "ProductId")]
[Table("sale_product")]
public partial class SaleProduct
{
    [Key]
    [Column("sale_id")]
    public int SaleId { get; set; }

    [Key]
    [Column("product_id")]
    public Guid ProductId { get; set; }

    [ForeignKey("SaleId")]
    [InverseProperty("SaleProducts")]
    public virtual Sale Sale { get; set; } = null!;
}
