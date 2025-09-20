using System;
using System.Collections.Generic;

namespace API_PUBLIC.Models;

public partial class User
{
    public Guid UserId { get; set; }

    public int? RoleId { get; set; }

    public bool? IsUserEnabled { get; set; }

    public string? UserGoogleId { get; set; }

    public string? UserUsername { get; set; }

    public string? UserPassword { get; set; }

    public string? UserFullname { get; set; }

    public string? UserEmail { get; set; }

    public string? UserPhone { get; set; }

    public string? UserBirthdate { get; set; }

    public string? UserAddress { get; set; }

    public string? UserImage { get; set; }

    public string? UserCurrentPaymentMethod { get; set; }

    public DateTimeOffset? UserCreatedAt { get; set; }

    public DateTimeOffset? UserUpdatedAt { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Order> OrderInchargeEmployees { get; set; } = new List<Order>();

    public virtual ICollection<Order> OrderUsers { get; set; } = new List<Order>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual Role? Role { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual ICollection<UserDiscount> UserDiscounts { get; set; } = new List<UserDiscount>();

    public virtual UserRank? UserRank { get; set; }
}
