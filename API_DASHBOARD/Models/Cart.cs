using System;
using System.Collections.Generic;

namespace API_DASHBOARD.Models;

public partial class Cart
{
    public Guid UserId { get; set; }

    public int VariationId { get; set; }

    public int? CartQuantity { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual Variation Variation { get; set; } = null!;
}
