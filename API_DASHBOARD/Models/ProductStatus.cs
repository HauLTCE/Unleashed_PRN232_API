using System;
using System.Collections.Generic;

namespace API_DASHBOARD.Models;

public partial class ProductStatus
{
    public int ProductStatusId { get; set; }

    public string? ProductStatusName { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
