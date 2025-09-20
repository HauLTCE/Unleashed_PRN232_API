using System;
using System.Collections.Generic;

namespace API_PUBLIC.Models;

public partial class Brand
{
    public int BrandId { get; set; }

    public string? BrandName { get; set; }

    public string? BrandDescription { get; set; }

    public string? BrandImageUrl { get; set; }

    public string? BrandWebsiteUrl { get; set; }

    public DateTimeOffset? BrandCreatedAt { get; set; }

    public DateTimeOffset? BrandUpdatedAt { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
