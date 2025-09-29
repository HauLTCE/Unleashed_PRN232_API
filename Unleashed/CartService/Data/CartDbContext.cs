using System;
using System.Collections.Generic;
using CartService.Models;
using Microsoft.EntityFrameworkCore;

namespace CartService.Data;

public partial class CartDbContext : DbContext
{

    public CartDbContext(DbContextOptions<CartDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<Wishlist> Wishlists { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.VariationId }).HasName("cart_pkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
