using System;
using System.Collections.Generic;

namespace API_DASHBOARD.Models;

public partial class Size
{
    public int SizeId { get; set; }

    public string? SizeName { get; set; }

    public virtual ICollection<Variation> Variations { get; set; } = new List<Variation>();
}
