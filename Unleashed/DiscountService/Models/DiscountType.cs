using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DiscountService.Models;

[Table("discount_type")]
public partial class DiscountType
{
    [Key]
    [Column("discount_type_id")]
    public int DiscountTypeId { get; set; }

    [Column("discount_type_name")]
    [StringLength(255)]
    public string? DiscountTypeName { get; set; }

    [InverseProperty("DiscountType")]
    public virtual ICollection<Discount> Discounts { get; set; } = new List<Discount>();
}
