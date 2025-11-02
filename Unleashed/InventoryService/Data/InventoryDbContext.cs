using System;
using System.Collections.Generic;
using InventoryService.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Data;

public partial class InventoryDbContext : DbContext
{

    public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Provider> Providers { get; set; }

    public virtual DbSet<Stock> Stocks { get; set; }

    public virtual DbSet<StockVariation> StockVariations { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<TransactionType> TransactionTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Provider>(entity =>
        {
            entity.HasKey(e => e.ProviderId).HasName("provider_pkey");
        });

        modelBuilder.Entity<Stock>(entity =>
        {
            entity.HasKey(e => e.StockId).HasName("stock_pkey");
        });

        modelBuilder.Entity<StockVariation>(entity =>
        {
            entity.HasKey(e => new { e.StockId, e.VariationId }).HasName("stock_variation_pkey");

            entity.HasOne(d => d.Stock).WithMany(p => p.StockVariations)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("stock_variation_stock_id_fkey");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("transaction_pkey");

            entity.HasOne(d => d.Provider).WithMany(p => p.Transactions).HasConstraintName("transaction_provider_id_fkey");

            entity.HasOne(d => d.Stock).WithMany(p => p.Transactions).OnDelete(DeleteBehavior.Cascade).HasConstraintName("transaction_stock_id_fkey");

            entity.HasOne(d => d.TransactionType).WithMany(p => p.Transactions).HasConstraintName("transaction_transaction_type_id_fkey");
        });

        modelBuilder.Entity<TransactionType>(entity =>
        {
            entity.HasKey(e => e.TransactionTypeId).HasName("transaction_type_pkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
