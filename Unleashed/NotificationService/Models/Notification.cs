using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NotificationService.Models;

[Table("notification")]
public partial class Notification
{
    [Key]
    [Column("notification_id")]
    public int NotificationId { get; set; }

    [Column("user_id_sender")]
    public Guid? UserIdSender { get; set; }

    [Column("notification_title")]
    [StringLength(255)]
    public string? NotificationTitle { get; set; }

    [Column("notification_content")]
    public string? NotificationContent { get; set; }

    [Column("is_notification_draft")]
    public bool? IsNotificationDraft { get; set; }

    [Column("notification_created_at")]
    public DateTimeOffset? NotificationCreatedAt { get; set; }

    [Column("notification_updated_at")]
    public DateTimeOffset? NotificationUpdatedAt { get; set; }
    public virtual ICollection<NotificationUser> NotificationUsers { get; set; } = [];

}
