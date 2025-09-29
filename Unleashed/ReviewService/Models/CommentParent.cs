using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ReviewService.Models;

[Keyless]
[Table("comment_parent")]
public partial class CommentParent
{
    [Column("comment_id")]
    public int? CommentId { get; set; }

    [Column("comment_parent_id")]
    public int? CommentParentId { get; set; }

    [ForeignKey("CommentId")]
    public virtual Comment? Comment { get; set; }

    [ForeignKey("CommentParentId")]
    public virtual Comment? CommentParentNavigation { get; set; }
}
