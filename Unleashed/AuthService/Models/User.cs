using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Models;

[Table("user")]
public partial class User
{
    [Key]
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("role_id")]
    public int? RoleId { get; set; }

    [Column("is_user_enabled")]
    public bool? IsUserEnabled { get; set; } = true;

    [Column("user_google_id")]
    [StringLength(255)]
    public string? UserGoogleId { get; set; }

    [Column("user_username")]
    [StringLength(255)]
    public string? UserUsername { get; set; }

    [Column("user_password")]
    [StringLength(255)]
    public string? UserPassword { get; set; }

    [Column("user_fullname")]
    [StringLength(255)]
    public string? UserFullname { get; set; }

    [Column("user_email")]
    [StringLength(255)]
    public string? UserEmail { get; set; }

    [Column("user_phone")]
    [StringLength(12)]
    public string? UserPhone { get; set; }

    [Column("user_birthdate")]
    [StringLength(255)]
    public string? UserBirthdate { get; set; }

    [Column("user_address")]
    [StringLength(255)]
    public string? UserAddress { get; set; }

    [Column("user_image")]
    [StringLength(255)]
    public string? UserImage { get; set; }

    [Column("user_current_payment_method")]
    [StringLength(255)]
    public string? UserCurrentPaymentMethod { get; set; }

    [Column("user_created_at")]
    public DateTimeOffset? UserCreatedAt { get; set; }

    [Column("user_updated_at")]
    public DateTimeOffset? UserUpdatedAt { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("Users")]
    public virtual Role? Role { get; set; }

    [InverseProperty("User")]
    public virtual UserRank? UserRank { get; set; }
}
