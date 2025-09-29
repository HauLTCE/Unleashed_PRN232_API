using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProductService.Models;

[Keyless]
[Table("product_category")]
public partial class ProductCategory
{
    [Column("product_id")]
    public Guid? ProductId { get; set; }

    [Column("category_id")]
    public int? CategoryId { get; set; }

    [ForeignKey("CategoryId")]
    public virtual Category? Category { get; set; }

    [ForeignKey("ProductId")]
    public virtual Product? Product { get; set; }
}
