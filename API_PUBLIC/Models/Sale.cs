using System;
using System.Collections.Generic;

namespace API_PUBLIC.Models;

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

    public virtual SaleStatus? SaleStatus { get; set; }

    public virtual SaleType? SaleType { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
