using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProductService.Models;

[Table("size")]
public partial class Size
{
    [Key]
    [Column("size_id")]
    public int SizeId { get; set; }

    [Column("size_name")]
    [StringLength(255)]
    public string? SizeName { get; set; }

    [InverseProperty("Size")]
    public virtual ICollection<Variation> Variations { get; set; } = new List<Variation>();
}
