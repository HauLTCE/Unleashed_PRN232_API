using System;
using System.Collections.Generic;

namespace ProductService.Models;

public partial class Review
{
    public int ReviewId { get; set; }

    public Guid? ProductId { get; set; }

    public Guid? UserId { get; set; }

    public string? OrderId { get; set; }

    public int? ReviewRating { get; set; }
}
