using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ReviewService.Models;

namespace ReviewService.Data;

public partial class ReviewDbContext : DbContext
{

    public ReviewDbContext(DbContextOptions<ReviewDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<CommentParent> CommentParents { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("comment_pkey");

            entity.HasOne(d => d.Review).WithMany(p => p.Comments).HasConstraintName("comment_review_id_fkey");
        });

        modelBuilder.Entity<CommentParent>(entity =>
        {
            entity.HasKey(cp => new { cp.CommentId, cp.CommentParentId });

            // This relationship represents the child comment in the pair
            entity.HasOne(d => d.Comment)
                  .WithMany()
                  .HasForeignKey(d => d.CommentId) // Explicitly define the foreign key
                  .HasConstraintName("comment_parent_comment_id_fkey")
                  .OnDelete(DeleteBehavior.Cascade); // Keep cascade for this one

            // This relationship represents the parent comment in the pair
            entity.HasOne(d => d.CommentParentNavigation)
                  .WithMany()
                  .HasForeignKey(d => d.CommentParentId) // Explicitly define the foreign key
                  .HasConstraintName("comment_parent_comment_parent_id_fkey")
                  .OnDelete(DeleteBehavior.NoAction); // THIS IS THE FIX: Break the cascade cycle
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("review_pkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}