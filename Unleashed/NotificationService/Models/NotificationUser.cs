using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NotificationService.Models;

[Keyless]
[Table("notification_user")]
public partial class NotificationUser
{
    [Column("notification_id")]
    public int? NotificationId { get; set; }

    [Column("user_id")]
    public Guid? UserId { get; set; }

    [Column("is_notification_viewed")]
    public bool? IsNotificationViewed { get; set; }

    [Column("is_notification_deleted")]
    public bool? IsNotificationDeleted { get; set; }

    [ForeignKey("NotificationId")]
    public virtual Notification? Notification { get; set; }
}
