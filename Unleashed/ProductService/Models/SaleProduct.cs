using System;
using System.Collections.Generic;

namespace ProductService.Models;

public partial class SaleProduct
{
    public int SaleId { get; set; }

    public Guid ProductId { get; set; }
}
