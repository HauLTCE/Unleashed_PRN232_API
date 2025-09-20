using System;
using System.Collections.Generic;

namespace API_PUBLIC.Models;

public partial class Rank
{
    public int RankId { get; set; }

    public string? RankName { get; set; }

    public int? RankNum { get; set; }

    public decimal? RankPaymentRequirement { get; set; }

    public decimal? RankBaseDiscount { get; set; }

    public virtual ICollection<Discount> Discounts { get; set; } = new List<Discount>();

    public virtual ICollection<UserRank> UserRanks { get; set; } = new List<UserRank>();
}
