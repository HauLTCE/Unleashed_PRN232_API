using System;
using System.Collections.Generic;

namespace ProductService.Models;

public partial class Sale
{
    public int SaleId { get; set; }

    public int? SaleTypeId { get; set; }

    public int? SaleStatusId { get; set; }

    public decimal? SaleValue { get; set; }

    public DateTimeOffset? SaleStartDate { get; set; }

    public DateTimeOffset? SaleEndDate { get; set; }

    public DateTimeOffset? SaleCreatedAt { get; set; }

    public DateTimeOffset? SaleUpdatedAt { get; set; }
}
