using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OrderService.Models;

[Table("shipping_method")]
public partial class ShippingMethod
{
    [Key]
    [Column("shipping_method_id")]
    public int ShippingMethodId { get; set; }

    [Column("shipping_method_name")]
    [StringLength(255)]
    public string? ShippingMethodName { get; set; }

    [InverseProperty("ShippingMethod")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
