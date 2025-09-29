using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProductService.Models;

[Table("product_status")]
public partial class ProductStatus
{
    [Key]
    [Column("product_status_id")]
    public int ProductStatusId { get; set; }

    [Column("product_status_name")]
    [StringLength(255)]
    public string? ProductStatusName { get; set; }

    [InverseProperty("ProductStatus")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
