using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProductService.Models;

[Table("brand")]
public partial class Brand
{
    [Key]
    [Column("brand_id")]
    public int BrandId { get; set; }

    [Column("brand_name")]
    [StringLength(255)]
    public string? BrandName { get; set; }

    [Column("brand_description")]
    [StringLength(255)]
    public string? BrandDescription { get; set; }

    [Column("brand_image_url")]
    [StringLength(255)]
    public string? BrandImageUrl { get; set; }

    [Column("brand_website_url")]
    [StringLength(255)]
    public string? BrandWebsiteUrl { get; set; }

    [Column("brand_created_at")]
    public DateTimeOffset? BrandCreatedAt { get; set; }

    [Column("brand_updated_at")]
    public DateTimeOffset? BrandUpdatedAt { get; set; }

    [InverseProperty("Brand")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
