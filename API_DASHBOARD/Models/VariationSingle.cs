using System;
using System.Collections.Generic;

namespace API_DASHBOARD.Models;

public partial class VariationSingle
{
    public int VariationSingleId { get; set; }

    public string? VariationSingleCode { get; set; }

    public bool? IsVariationSingleBought { get; set; }

    public int? VariationId { get; set; }

    public virtual ICollection<OrderVariationSingle> OrderVariationSingles { get; set; } = new List<OrderVariationSingle>();

    public virtual Variation? Variation { get; set; }
}
