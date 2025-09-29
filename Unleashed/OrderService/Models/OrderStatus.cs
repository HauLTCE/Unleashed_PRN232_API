using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OrderService.Models;

[Table("order_status")]
public partial class OrderStatus
{
    [Key]
    [Column("order_status_id")]
    public int OrderStatusId { get; set; }

    [Column("order_status_name")]
    [StringLength(255)]
    public string? OrderStatusName { get; set; }

    [InverseProperty("OrderStatus")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
