using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DiscountService.Models;

[Table("discount")]
public partial class Discount
{
    [Key]
    [Column("discount_id")]
    public int DiscountId { get; set; }

    [Column("discount_status_id")]
    public int? DiscountStatusId { get; set; }

    [Column("discount_type_id")]
    public int? DiscountTypeId { get; set; }

    [Column("discount_code")]
    [StringLength(20)]
    public string DiscountCode { get; set; } = null!;

    [Column("discount_value", TypeName = "decimal(22, 2)")]
    public decimal? DiscountValue { get; set; }

    [Column("discount_description")]
    [StringLength(255)]
    public string? DiscountDescription { get; set; }

    [Column("discount_rank_requirement")]
    public int? DiscountRankRequirement { get; set; }

    [Column("discount_minimum_order_value", TypeName = "decimal(22, 2)")]
    public decimal? DiscountMinimumOrderValue { get; set; }

    [Column("discount_maximum_value", TypeName = "decimal(22, 2)")]
    public decimal? DiscountMaximumValue { get; set; }

    [Column("discount_usage_limit")]
    public int? DiscountUsageLimit { get; set; }

    [Column("discount_start_date")]
    public DateTimeOffset? DiscountStartDate { get; set; }

    [Column("discount_end_date")]
    public DateTimeOffset? DiscountEndDate { get; set; }

    [Column("discount_created_at")]
    public DateTimeOffset? DiscountCreatedAt { get; set; }

    [Column("discount_updated_at")]
    public DateTimeOffset? DiscountUpdatedAt { get; set; }

    [Column("discount_usage_count")]
    public int? DiscountUsageCount { get; set; }

    [ForeignKey("DiscountStatusId")]
    [InverseProperty("Discounts")]
    public virtual DiscountStatus? DiscountStatus { get; set; }

    [ForeignKey("DiscountTypeId")]
    [InverseProperty("Discounts")]
    public virtual DiscountType? DiscountType { get; set; }

    [InverseProperty("Discount")]
    public virtual ICollection<UserDiscount> UserDiscounts { get; set; } = new List<UserDiscount>();
}
