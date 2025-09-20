using System;
using System.Collections.Generic;

namespace API_DASHBOARD.Models;

public partial class Discount
{
    public int DiscountId { get; set; }

    public int? DiscountStatusId { get; set; }

    public int? DiscountTypeId { get; set; }

    public string DiscountCode { get; set; } = null!;

    public decimal? DiscountValue { get; set; }

    public string? DiscountDescription { get; set; }

    public int? DiscountRankRequirement { get; set; }

    public decimal? DiscountMinimumOrderValue { get; set; }

    public decimal? DiscountMaximumValue { get; set; }

    public int? DiscountUsageLimit { get; set; }

    public DateTimeOffset? DiscountStartDate { get; set; }

    public DateTimeOffset? DiscountEndDate { get; set; }

    public DateTimeOffset? DiscountCreatedAt { get; set; }

    public DateTimeOffset? DiscountUpdatedAt { get; set; }

    public int? DiscountUsageCount { get; set; }

    public virtual Rank? DiscountRankRequirementNavigation { get; set; }

    public virtual DiscountStatus? DiscountStatus { get; set; }

    public virtual DiscountType? DiscountType { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<UserDiscount> UserDiscounts { get; set; } = new List<UserDiscount>();
}
