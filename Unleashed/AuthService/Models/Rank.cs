using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Models;

[Table("rank")]
public partial class Rank
{
    [Key]
    [Column("rank_id")]
    public int RankId { get; set; }

    [Column("rank_name")]
    public string? RankName { get; set; }

    [Column("rank_num")]
    public int? RankNum { get; set; }

    [Column("rank_payment_requirement", TypeName = "decimal(22, 2)")]
    public decimal? RankPaymentRequirement { get; set; }

    [Column("rank_base_discount", TypeName = "numeric(3, 2)")]
    public decimal? RankBaseDiscount { get; set; }

    [InverseProperty("Rank")]
    public virtual ICollection<UserRank> UserRanks { get; set; } = new List<UserRank>();
}
