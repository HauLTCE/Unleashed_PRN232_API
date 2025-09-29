using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OrderService.Models;

[Table("payment_method")]
public partial class PaymentMethod
{
    [Key]
    [Column("payment_method_id")]
    public int PaymentMethodId { get; set; }

    [Column("payment_method_name")]
    [StringLength(255)]
    public string? PaymentMethodName { get; set; }

    [InverseProperty("PaymentMethod")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
