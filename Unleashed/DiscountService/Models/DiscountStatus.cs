using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DiscountService.Models;

[Table("discount_status")]
public partial class DiscountStatus
{
    [Key]
    [Column("discount_status_id")]
    public int DiscountStatusId { get; set; }

    [Column("discount_status_name")]
    [StringLength(255)]
    public string? DiscountStatusName { get; set; }

    [InverseProperty("DiscountStatus")]
    public virtual ICollection<Discount> Discounts { get; set; } = new List<Discount>();
}
