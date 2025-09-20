using System;
using System.Collections.Generic;

namespace API_HOMEPAGE.Models;

public partial class UserRank
{
    public Guid UserId { get; set; }

    public int RankId { get; set; }

    public decimal? MoneySpent { get; set; }

    public short RankStatus { get; set; }

    public DateOnly RankExpireDate { get; set; }

    public DateTimeOffset? RankCreatedDate { get; set; }

    public DateTimeOffset? RankUpdatedDate { get; set; }

    public virtual Rank Rank { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
