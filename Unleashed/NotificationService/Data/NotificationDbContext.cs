using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NotificationService.Models;

namespace NotificationService.Data;

public partial class NotificationDbContext : DbContext
{

    public NotificationDbContext(DbContextOptions<NotificationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<NotificationUser> NotificationUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("notification_pkey");
        });

        modelBuilder.Entity<NotificationUser>(entity =>
        {
            entity.HasOne(d => d.Notification).WithMany().HasConstraintName("notification_user_notification_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
