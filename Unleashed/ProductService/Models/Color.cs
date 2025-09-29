using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProductService.Models;

[Table("color")]
public partial class Color
{
    [Key]
    [Column("color_id")]
    public int ColorId { get; set; }

    [Column("color_name")]
    [StringLength(255)]
    public string? ColorName { get; set; }

    [Column("color_hex_code")]
    [StringLength(255)]
    public string? ColorHexCode { get; set; }

    [InverseProperty("Color")]
    public virtual ICollection<Variation> Variations { get; set; } = new List<Variation>();
}
