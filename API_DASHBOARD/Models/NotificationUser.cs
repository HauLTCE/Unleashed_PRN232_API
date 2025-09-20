using System;
using System.Collections.Generic;

namespace API_DASHBOARD.Models;

public partial class NotificationUser
{
    public int? NotificationId { get; set; }

    public Guid? UserId { get; set; }

    public bool? IsNotificationViewed { get; set; }

    public bool? IsNotificationDeleted { get; set; }

    public virtual Notification? Notification { get; set; }

    public virtual User? User { get; set; }
}
