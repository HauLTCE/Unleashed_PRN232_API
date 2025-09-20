using System;
using System.Collections.Generic;

namespace API_PUBLIC.Models;

public partial class CommentParent
{
    public int? CommentId { get; set; }

    public int? CommentParentId { get; set; }

    public virtual Comment? Comment { get; set; }

    public virtual Comment? CommentParentNavigation { get; set; }
}
