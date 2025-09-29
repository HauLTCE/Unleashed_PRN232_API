using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ReviewService.Models;

[Table("review")]
public partial class Review
{
    [Key]
    [Column("review_id")]
    public int ReviewId { get; set; }

    [Column("product_id")]
    public Guid? ProductId { get; set; }

    [Column("user_id")]
    public Guid? UserId { get; set; }

    [Column("order_id")]
    [StringLength(255)]
    public string? OrderId { get; set; }

    [Column("review_rating")]
    public int? ReviewRating { get; set; }

    [InverseProperty("Review")]
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
