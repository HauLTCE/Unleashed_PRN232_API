using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CartService.Models;

[Keyless]
[Table("wishlist")]
public partial class Wishlist
{
    [Column("user_id")]
    public Guid? UserId { get; set; }

    [Column("product_id")]
    public Guid? ProductId { get; set; }
}
