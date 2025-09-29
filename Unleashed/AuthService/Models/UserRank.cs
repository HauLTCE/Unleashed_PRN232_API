using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Models;

[Table("user_rank")]
public partial class UserRank
{
    [Key]
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("rank_id")]
    public int RankId { get; set; }

    [Column("money_spent", TypeName = "decimal(22, 2)")]
    public decimal? MoneySpent { get; set; }

    [Column("rank_status")]
    public short RankStatus { get; set; }

    [Column("rank_expire_date")]
    public DateOnly RankExpireDate { get; set; }

    [Column("rank_created_date")]
    public DateTimeOffset? RankCreatedDate { get; set; }

    [Column("rank_updated_date")]
    public DateTimeOffset? RankUpdatedDate { get; set; }

    [ForeignKey("RankId")]
    [InverseProperty("UserRanks")]
    public virtual Rank Rank { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("UserRank")]
    public virtual User User { get; set; } = null!;
}
