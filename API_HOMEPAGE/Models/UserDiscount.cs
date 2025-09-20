using System;
using System.Collections.Generic;

namespace API_HOMEPAGE.Models;

public partial class UserDiscount
{
    public Guid UserId { get; set; }

    public int DiscountId { get; set; }

    public bool IsDiscountUsed { get; set; }

    public DateTimeOffset? DiscountUsedAt { get; set; }

    public virtual Discount Discount { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
