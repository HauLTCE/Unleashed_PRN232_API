using System;
using System.Collections.Generic;

namespace API_HOMEPAGE.Models;

public partial class Color
{
    public int ColorId { get; set; }

    public string? ColorName { get; set; }

    public string? ColorHexCode { get; set; }

    public virtual ICollection<Variation> Variations { get; set; } = new List<Variation>();
}
