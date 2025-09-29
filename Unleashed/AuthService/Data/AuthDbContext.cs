using System;
using System.Collections.Generic;
using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data;

public partial class AuthDbContext : DbContext
{

    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Rank> Ranks { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRank> UserRanks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Rank>(entity =>
        {
            entity.HasKey(e => e.RankId).HasName("rank_pkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("role_pkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("user_pkey");

            entity.Property(e => e.UserId).ValueGeneratedNever();

            entity.HasOne(d => d.Role).WithMany(p => p.Users).HasConstraintName("user_role_id_fkey");
        });

        modelBuilder.Entity<UserRank>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("user_rank_pkey");

            entity.Property(e => e.UserId).ValueGeneratedNever();

            entity.HasOne(d => d.Rank).WithMany(p => p.UserRanks)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_rank_rank_id_fkey");

            entity.HasOne(d => d.User).WithOne(p => p.UserRank)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_rank_user_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
