using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ReviewService.Models;

[Table("comment")]
public partial class Comment
{
    [Key]
    [Column("comment_id")]
    public int CommentId { get; set; }

    [Column("user_id")]
    public Guid? UserId { get; set; }
    [Column("review_id")]
    public int? ReviewId { get; set; }

    [Column("comment_content")]
    [StringLength(255)]
    public string? CommentContent { get; set; }

    [Column("comment_created_at")]
    public DateTimeOffset? CommentCreatedAt { get; set; }

    [Column("comment_updated_at")]
    public DateTimeOffset? CommentUpdatedAt { get; set; }

    [ForeignKey("ReviewId")]
    [InverseProperty("Comments")]
    public virtual Review? Review { get; set; }
}
