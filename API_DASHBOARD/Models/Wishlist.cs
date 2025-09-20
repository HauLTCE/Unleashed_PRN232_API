using System;
using System.Collections.Generic;

namespace API_DASHBOARD.Models;

public partial class Wishlist
{
    public Guid? UserId { get; set; }

    public Guid? ProductId { get; set; }

    public virtual Product? Product { get; set; }

    public virtual User? User { get; set; }
}
