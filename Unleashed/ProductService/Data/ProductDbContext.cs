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
    public virtual DbSet<Review> Reviews { get; set; }
    public virtual DbSet<Sale> Sales { get; set; }
    public virtual DbSet<SaleProduct> SaleProducts { get; set; }
    public virtual DbSet<StockVariation> StockVariations { get; set; }

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
        // =============== REVIEW ===============
        modelBuilder.Entity<Review>(entity =>
        {
            entity.ToTable("review");
            entity.HasKey(e => e.ReviewId).HasName("review_pkey");

            // Tuỳ DB có/không FK tới Product:
            entity.Property(e => e.ReviewId).HasColumnName("review_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ReviewRating).HasColumnName("review_rating");

            // Nếu muốn nối Product (đã có entity Product):
            entity.HasOne<Product>()
                  .WithMany()
                  .HasForeignKey(e => e.ProductId)
                  .HasConstraintName("review_product_id_fkey")
                  .OnDelete(DeleteBehavior.Restrict);
            // Nếu DB CHƯA có ràng buộc FK hoặc khác tên, có thể comment block trên.
        });

        // =============== SALE ===============
        modelBuilder.Entity<Sale>(entity =>
        {
            entity.ToTable("sale");
            entity.HasKey(e => e.SaleId).HasName("sale_pkey");

            entity.Property(e => e.SaleId).HasColumnName("sale_id");
            entity.Property(e => e.SaleTypeId).HasColumnName("sale_type_id");
            entity.Property(e => e.SaleStatusId).HasColumnName("sale_status_id");
            entity.Property(e => e.SaleValue)
                  .HasColumnName("sale_value")
                  .HasPrecision(18, 2); // tuỳ DB
            entity.Property(e => e.SaleStartDate).HasColumnName("sale_start_date");
            entity.Property(e => e.SaleEndDate).HasColumnName("sale_end_date");
            entity.Property(e => e.SaleCreatedAt).HasColumnName("sale_created_at");
            entity.Property(e => e.SaleUpdatedAt).HasColumnName("sale_updated_at");

            // Nếu Sếp scaffold thêm hai bảng sale_status & sale_type (khuyến nghị),
            // thêm quan hệ như dưới đây. Nếu CHƯA có model thì comment lại.
            // entity.HasOne<SaleStatus>()
            //       .WithMany()
            //       .HasForeignKey(e => e.SaleStatusId)
            //       .HasConstraintName("sale_sale_status_id_fkey")
            //       .OnDelete(DeleteBehavior.Restrict);

            // entity.HasOne<SaleType>()
            //       .WithMany()
            //       .HasForeignKey(e => e.SaleTypeId)
            //       .HasConstraintName("sale_sale_type_id_fkey")
            //       .OnDelete(DeleteBehavior.Restrict);
        });

        // =============== SALE_PRODUCT (Bridge) ===============
        modelBuilder.Entity<SaleProduct>(entity =>
        {
            entity.ToTable("sale_product");

            // Bảng nối: dùng composite key (SaleId, ProductId)
            entity.HasKey(e => new { e.SaleId, e.ProductId })
                  .HasName("sale_product_pkey");

            entity.Property(e => e.SaleId).HasColumnName("sale_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");

            entity.HasOne<Sale>()
                  .WithMany()
                  .HasForeignKey(e => e.SaleId)
                  .HasConstraintName("sale_product_sale_id_fkey")
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<Product>()
                  .WithMany()
                  .HasForeignKey(e => e.ProductId)
                  .HasConstraintName("sale_product_product_id_fkey")
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // =============== STOCK_VARIATION ===============
        modelBuilder.Entity<StockVariation>(entity =>
        {
            entity.ToTable("stock_variation");

            // Không có khoá đơn lẻ, ta đặt composite key theo hai cột hiện có:
            entity.HasKey(e => new { e.VariationId, e.StockId })
                  .HasName("stock_variation_pkey");

            entity.Property(e => e.VariationId).HasColumnName("variation_id");
            entity.Property(e => e.StockId).HasColumnName("stock_id");
            entity.Property(e => e.StockQuantity).HasColumnName("stock_quantity");

            // Liên kết Variation (đã có entity Variation)
            entity.HasOne<Variation>()
                  .WithMany()
                  .HasForeignKey(e => e.VariationId)
                  .HasConstraintName("stock_variation_variation_id_fkey")
                  .OnDelete(DeleteBehavior.Cascade);

            // Nếu Sếp có entity Stock thì mở khoá quan hệ dưới đây:
            // entity.HasOne<Stock>()
            //       .WithMany()
            //       .HasForeignKey(e => e.StockId)
            //       .HasConstraintName("stock_variation_stock_id_fkey")
            //       .OnDelete(DeleteBehavior.Cascade);
        });
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
