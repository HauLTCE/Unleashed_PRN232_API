using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Models;

[Table("stock")]
public partial class Stock
{
    [Key]
    [Column("stock_id")]
    public int StockId { get; set; }

    [Column("stock_name")]
    [StringLength(255)]
    public string? StockName { get; set; }

    [Column("stock_address")]
    [StringLength(255)]
    public string? StockAddress { get; set; }

    [InverseProperty("Stock")]
    public virtual ICollection<StockVariation> StockVariations { get; set; } = new List<StockVariation>();

    [InverseProperty("Stock")]
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
