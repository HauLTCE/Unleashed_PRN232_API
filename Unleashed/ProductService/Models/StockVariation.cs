using System;
using System.Collections.Generic;

namespace ProductService.Models;

public partial class StockVariation
{
    public int VariationId { get; set; }

    public int StockId { get; set; }

    public int? StockQuantity { get; set; }
}
