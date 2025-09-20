using System;
using System.Collections.Generic;

namespace API_HOMEPAGE.Models;

public partial class Comment
{
    public int CommentId { get; set; }

    public int? ReviewId { get; set; }

    public string? CommentContent { get; set; }

    public DateTimeOffset? CommentCreatedAt { get; set; }

    public DateTimeOffset? CommentUpdatedAt { get; set; }

    public virtual Review? Review { get; set; }
}
