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
            entity.HasOne(d => d.Comment).WithMany().HasConstraintName("comment_parent_comment_id_fkey");

            entity.HasOne(d => d.CommentParentNavigation).WithMany().HasConstraintName("comment_parent_comment_parent_id_fkey");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("review_pkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
