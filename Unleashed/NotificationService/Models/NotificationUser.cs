using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotificationService.Models;

[Table("notification_user")]
public partial class NotificationUser
{
    [Key]
    [Column("notification_id", Order = 0)]
    public int NotificationId { get; set; }

    [Key]
    [Column("user_id", Order = 1)]
    public Guid UserId { get; set; }

    [Column("is_notification_viewed")]
    public bool? IsNotificationViewed { get; set; } = false;

    [Column("is_notification_deleted")]
    public bool? IsNotificationDeleted { get; set; } = false;

    [ForeignKey(nameof(NotificationId))]
    public virtual Notification? Notification { get; set; }
}
