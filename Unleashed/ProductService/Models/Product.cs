using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProductService.Models;

[Table("product")]
public partial class Product
{
    [Key]
    [Column("product_id")]
    public Guid ProductId { get; set; }

    [Column("brand_id")]
    public int? BrandId { get; set; }

    [Column("product_status_id")]
    public int? ProductStatusId { get; set; }

    [Column("product_name")]
    [StringLength(255)]
    public string? ProductName { get; set; }

    [Column("product_code")]
    [StringLength(255)]
    public string? ProductCode { get; set; }

    [Column("product_description")]
    public string? ProductDescription { get; set; }

    [Column("product_created_at")]
    public DateTimeOffset? ProductCreatedAt { get; set; }

    [Column("product_updated_at")]
    public DateTimeOffset? ProductUpdatedAt { get; set; }

    [ForeignKey("BrandId")]
    [InverseProperty("Products")]
    public virtual Brand? Brand { get; set; }

    [ForeignKey("ProductStatusId")]
    [InverseProperty("Products")]
    public virtual ProductStatus? ProductStatus { get; set; }

    [InverseProperty("Product")]
    public virtual ICollection<Variation> Variations { get; set; } = new List<Variation>();

    [InverseProperty("Product")]
    public virtual ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
}
