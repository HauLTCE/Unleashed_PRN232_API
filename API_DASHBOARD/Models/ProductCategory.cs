using System;
using System.Collections.Generic;

namespace API_DASHBOARD.Models;

public partial class ProductCategory
{
    public Guid? ProductId { get; set; }

    public int? CategoryId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual Product? Product { get; set; }
}
