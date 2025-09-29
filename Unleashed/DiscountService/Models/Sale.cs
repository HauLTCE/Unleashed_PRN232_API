using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DiscountService.Models;

[Table("sale")]
public partial class Sale
{
    [Key]
    [Column("sale_id")]
    public int SaleId { get; set; }

    [Column("sale_type_id")]
    public int? SaleTypeId { get; set; }

    [Column("sale_status_id")]
    public int? SaleStatusId { get; set; }

    [Column("sale_value", TypeName = "decimal(22, 2)")]
    public decimal? SaleValue { get; set; }

    [Column("sale_start_date")]
    public DateTimeOffset? SaleStartDate { get; set; }

    [Column("sale_end_date")]
    public DateTimeOffset? SaleEndDate { get; set; }

    [Column("sale_created_at")]
    public DateTimeOffset? SaleCreatedAt { get; set; }

    [Column("sale_updated_at")]
    public DateTimeOffset? SaleUpdatedAt { get; set; }

    [InverseProperty("Sale")]
    public virtual ICollection<SaleProduct> SaleProducts { get; set; } = new List<SaleProduct>();

    [ForeignKey("SaleStatusId")]
    [InverseProperty("Sales")]
    public virtual SaleStatus? SaleStatus { get; set; }

    [ForeignKey("SaleTypeId")]
    [InverseProperty("Sales")]
    public virtual SaleType? SaleType { get; set; }
}
