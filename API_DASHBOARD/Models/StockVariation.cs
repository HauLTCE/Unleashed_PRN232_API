using System;
using System.Collections.Generic;

namespace API_DASHBOARD.Models;

public partial class StockVariation
{
    public int VariationId { get; set; }

    public int StockId { get; set; }

    public int? StockQuantity { get; set; }

    public virtual Stock Stock { get; set; } = null!;

    public virtual Variation Variation { get; set; } = null!;
}
