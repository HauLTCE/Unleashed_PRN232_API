using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CartService.Models;

[PrimaryKey("UserId", "VariationId")]
[Table("cart")]
public partial class Cart
{
    [Key]
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Key]
    [Column("variation_id")]
    public int VariationId { get; set; }

    [Column("cart_quantity")]
    public int? CartQuantity { get; set; }
}
