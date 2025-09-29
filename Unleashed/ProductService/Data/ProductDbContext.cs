using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ProductService.Models;

namespace ProductService.Data;

public partial class ProductDbContext : DbContext
{

    public ProductDbContext(DbContextOptions<ProductDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Color> Colors { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductCategory> ProductCategories { get; set; }

    public virtual DbSet<ProductStatus> ProductStatuses { get; set; }

    public virtual DbSet<Size> Sizes { get; set; }

    public virtual DbSet<Variation> Variations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.BrandId).HasName("brand_pkey");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("category_pkey");
        });

        modelBuilder.Entity<Color>(entity =>
        {
            entity.HasKey(e => e.ColorId).HasName("color_pkey");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("product_pkey");

            entity.Property(e => e.ProductId).ValueGeneratedNever();

            entity.HasOne(d => d.Brand).WithMany(p => p.Products).HasConstraintName("product_brand_id_fkey");

            entity.HasOne(d => d.ProductStatus).WithMany(p => p.Products).HasConstraintName("product_product_status_id_fkey");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasOne(d => d.Category).WithMany().HasConstraintName("product_category_category_id_fkey");

            entity.HasOne(d => d.Product).WithMany().HasConstraintName("product_category_product_id_fkey");
        });

        modelBuilder.Entity<ProductStatus>(entity =>
        {
            entity.HasKey(e => e.ProductStatusId).HasName("product_status_pkey");
        });

        modelBuilder.Entity<Size>(entity =>
        {
            entity.HasKey(e => e.SizeId).HasName("size_pkey");
        });

        modelBuilder.Entity<Variation>(entity =>
        {
            entity.HasKey(e => e.VariationId).HasName("PK_variation_temp");

            entity.HasOne(d => d.Color).WithMany(p => p.Variations).HasConstraintName("FK9gqn7oby75ixq0fg8w902pjcf");

            entity.HasOne(d => d.Product).WithMany(p => p.Variations).HasConstraintName("FK1hxfv06p366bhb8sce1djt2v7");

            entity.HasOne(d => d.Size).WithMany(p => p.Variations).HasConstraintName("FKe3asl55h6omj6x27479hnprov");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
