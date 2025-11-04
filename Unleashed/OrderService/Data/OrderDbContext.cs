using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using OrderService.Models;

namespace OrderService.Data;

public partial class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<OrderVariation> OrderVariationSingles { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<ShippingMethod> ShippingMethods { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("order_pkey");

            entity.HasOne(d => d.OrderStatus).WithMany(p => p.Orders).HasConstraintName("order_order_status_id_fkey");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Orders).HasConstraintName("order_payment_method_id_fkey");

            entity.HasOne(d => d.ShippingMethod).WithMany(p => p.Orders).HasConstraintName("order_shipping_method_id_fkey");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.OrderStatusId).HasName("order_status_pkey");
            entity.HasData(
                new OrderStatus { OrderStatusId = 1, OrderStatusName = "PENDING" },
                new OrderStatus { OrderStatusId = 2, OrderStatusName = "PROCESSING" },
                new OrderStatus { OrderStatusId = 3, OrderStatusName = "SHIPPING" },
                new OrderStatus { OrderStatusId = 4, OrderStatusName = "COMPLETED" },
                new OrderStatus { OrderStatusId = 5, OrderStatusName = "CANCELLED" },
                new OrderStatus { OrderStatusId = 6, OrderStatusName = "RETURNED" },
                new OrderStatus { OrderStatusId = 7, OrderStatusName = "DENIED" },
                new OrderStatus { OrderStatusId = 8, OrderStatusName = "INSPECTION" },
                new OrderStatus { OrderStatusId = 9, OrderStatusName = "RETURNING" }
            );
        });

        modelBuilder.Entity<OrderVariation>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.VariationId }).HasName("order_variation_pkey");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderVariations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_variation_id_fkey");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.PaymentMethodId).HasName("payment_method_pkey");
            entity.HasData(
                new PaymentMethod { PaymentMethodId = 1, PaymentMethodName = "COD" },
                new PaymentMethod { PaymentMethodId = 2, PaymentMethodName = "VNPAY" },
                new PaymentMethod { PaymentMethodId = 3, PaymentMethodName = "TRANSFER" }
            );
        });

        modelBuilder.Entity<ShippingMethod>(entity =>
        {
            entity.HasKey(e => e.ShippingMethodId).HasName("shipping_method_pkey");
            entity.HasData(
                new ShippingMethod { ShippingMethodId = 1, ShippingMethodName = "EXPRESS" },
                new ShippingMethod { ShippingMethodId = 2, ShippingMethodName = "STANDARD" }
            );
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
