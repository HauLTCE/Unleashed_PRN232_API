using System;
using System.Collections.Generic;

namespace API_PUBLIC.Models;

public partial class OrderStatus
{
    public int OrderStatusId { get; set; }

    public string? OrderStatusName { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
