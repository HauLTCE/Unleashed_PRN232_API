using System;
using System.Collections.Generic;
using DiscountService.Models;
using Microsoft.EntityFrameworkCore;

namespace DiscountService.Data;

public partial class DiscountDbContext : DbContext
{

    public DiscountDbContext(DbContextOptions<DiscountDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<DiscountStatus> DiscountStatuses { get; set; }

    public virtual DbSet<DiscountType> DiscountTypes { get; set; }

    public virtual DbSet<Sale> Sales { get; set; }

    public virtual DbSet<SaleProduct> SaleProducts { get; set; }

    public virtual DbSet<SaleStatus> SaleStatuses { get; set; }

    public virtual DbSet<SaleType> SaleTypes { get; set; }

    public virtual DbSet<UserDiscount> UserDiscounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.DiscountId).HasName("discount_pkey");

            entity.HasOne(d => d.DiscountStatus).WithMany(p => p.Discounts).HasConstraintName("discount_discount_status_id_fkey");

            entity.HasOne(d => d.DiscountType).WithMany(p => p.Discounts).HasConstraintName("discount_discount_type_id_fkey");
        });

        modelBuilder.Entity<DiscountStatus>(entity =>
        {
            entity.HasKey(e => e.DiscountStatusId).HasName("discount_status_pkey");
        });

        modelBuilder.Entity<DiscountType>(entity =>
        {
            entity.HasKey(e => e.DiscountTypeId).HasName("discount_type_pkey");
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(e => e.SaleId).HasName("sale_pkey");

            entity.HasOne(d => d.SaleStatus).WithMany(p => p.Sales).HasConstraintName("sale_sale_status_id_fkey");

            entity.HasOne(d => d.SaleType).WithMany(p => p.Sales).HasConstraintName("sale_sale_type_id_fkey");
        });

        modelBuilder.Entity<SaleProduct>(entity =>
        {
            entity.HasKey(e => new { e.SaleId, e.ProductId }).HasName("sale_product_pkey");

            entity.HasOne(d => d.Sale).WithMany(p => p.SaleProducts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sale_product_sale_id_fkey");
        });

        modelBuilder.Entity<SaleStatus>(entity =>
        {
            entity.HasKey(e => e.SaleStatusId).HasName("sale_status_pkey");
        });

        modelBuilder.Entity<SaleType>(entity =>
        {
            entity.HasKey(e => e.SaleTypeId).HasName("sale_type_pkey");
        });

        modelBuilder.Entity<UserDiscount>(entity =>
        {
            entity.HasKey(e => new { e.DiscountId, e.UserId }).HasName("user_discount_pkey");

            entity.HasOne(d => d.Discount).WithMany(p => p.UserDiscounts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_discount_discount_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
