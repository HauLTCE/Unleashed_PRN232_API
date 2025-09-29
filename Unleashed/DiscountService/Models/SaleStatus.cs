using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DiscountService.Models;

[Table("sale_status")]
public partial class SaleStatus
{
    [Key]
    [Column("sale_status_id")]
    public int SaleStatusId { get; set; }

    [Column("sale_status_name")]
    [StringLength(255)]
    public string? SaleStatusName { get; set; }

    [InverseProperty("SaleStatus")]
    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
