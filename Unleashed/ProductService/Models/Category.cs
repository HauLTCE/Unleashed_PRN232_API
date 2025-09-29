using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProductService.Models;

[Table("category")]
public partial class Category
{
    [Key]
    [Column("category_id")]
    public int CategoryId { get; set; }

    [Column("category_name")]
    [StringLength(255)]
    public string? CategoryName { get; set; }

    [Column("category_description")]
    public string? CategoryDescription { get; set; }

    [Column("category_image_url")]
    [StringLength(255)]
    public string? CategoryImageUrl { get; set; }

    [Column("category_created_at")]
    public DateTimeOffset? CategoryCreatedAt { get; set; }

    [Column("category_updated_at")]
    public DateTimeOffset? CategoryUpdatedAt { get; set; }
}
