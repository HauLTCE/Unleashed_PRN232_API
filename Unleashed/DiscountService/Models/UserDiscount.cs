using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DiscountService.Models;

[PrimaryKey("DiscountId", "UserId")]
[Table("user_discount")]
public partial class UserDiscount
{
    [Key]
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Key]
    [Column("discount_id")]
    public int DiscountId { get; set; }

    [Column("is_discount_used")]
    public bool IsDiscountUsed { get; set; }

    [Column("discount_used_at")]
    public DateTimeOffset? DiscountUsedAt { get; set; }

    [ForeignKey("DiscountId")]
    [InverseProperty("UserDiscounts")]
    public virtual Discount Discount { get; set; } = null!;
}
