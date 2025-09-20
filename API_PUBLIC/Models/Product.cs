using System;
using System.Collections.Generic;

namespace API_PUBLIC.Models;

public partial class Product
{
    public Guid ProductId { get; set; }

    public int? BrandId { get; set; }

    public int? ProductStatusId { get; set; }

    public string? ProductName { get; set; }

    public string? ProductCode { get; set; }

    public string? ProductDescription { get; set; }

    public DateTimeOffset? ProductCreatedAt { get; set; }

    public DateTimeOffset? ProductUpdatedAt { get; set; }

    public virtual Brand? Brand { get; set; }

    public virtual ProductStatus? ProductStatus { get; set; }

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Variation> Variations { get; set; } = new List<Variation>();

    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
