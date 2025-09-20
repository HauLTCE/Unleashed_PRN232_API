using System;
using System.Collections.Generic;

namespace API_HOMEPAGE.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public Guid? UserIdSender { get; set; }

    public string? NotificationTitle { get; set; }

    public string? NotificationContent { get; set; }

    public bool? IsNotificationDraft { get; set; }

    public DateTimeOffset? NotificationCreatedAt { get; set; }

    public DateTimeOffset? NotificationUpdatedAt { get; set; }

    public virtual User? UserIdSenderNavigation { get; set; }
}
