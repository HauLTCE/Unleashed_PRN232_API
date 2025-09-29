using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProductService.Models;

[Table("variation")]
public partial class Variation
{
    [Key]
    [Column("variation_id")]
    public int VariationId { get; set; }

    [Column("product_id")]
    public Guid? ProductId { get; set; }

    [Column("size_id")]
    public int? SizeId { get; set; }

    [Column("color_id")]
    public int? ColorId { get; set; }

    [Column("variation_image")]
    public string? VariationImage { get; set; }

    [Column("variation_price", TypeName = "decimal(22, 2)")]
    public decimal? VariationPrice { get; set; }

    [ForeignKey("ColorId")]
    [InverseProperty("Variations")]
    public virtual Color? Color { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("Variations")]
    public virtual Product? Product { get; set; }

    [ForeignKey("SizeId")]
    [InverseProperty("Variations")]
    public virtual Size? Size { get; set; }
}
