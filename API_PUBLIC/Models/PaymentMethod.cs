using System;
using System.Collections.Generic;

namespace API_PUBLIC.Models;

public partial class PaymentMethod
{
    public int PaymentMethodId { get; set; }

    public string? PaymentMethodName { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
