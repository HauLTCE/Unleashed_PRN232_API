using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DiscountService.Models;

[Table("sale_type")]
public partial class SaleType
{
    [Key]
    [Column("sale_type_id")]
    public int SaleTypeId { get; set; }

    [Column("sale_type_name")]
    [StringLength(255)]
    public string? SaleTypeName { get; set; }

    [InverseProperty("SaleType")]
    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
