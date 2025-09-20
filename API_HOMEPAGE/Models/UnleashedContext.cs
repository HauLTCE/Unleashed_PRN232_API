using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace API_HOMEPAGE.Models;

public partial class UnleashedContext : DbContext
{
    public UnleashedContext()
    {
    }

    public UnleashedContext(DbContextOptions<UnleashedContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Color> Colors { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<CommentParent> CommentParents { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<DiscountStatus> DiscountStatuses { get; set; }

    public virtual DbSet<DiscountType> DiscountTypes { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<NotificationUser> NotificationUsers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<OrderVariationSingle> OrderVariationSingles { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductCategory> ProductCategories { get; set; }

    public virtual DbSet<ProductStatus> ProductStatuses { get; set; }

    public virtual DbSet<Provider> Providers { get; set; }

    public virtual DbSet<Rank> Ranks { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Sale> Sales { get; set; }

    public virtual DbSet<SaleStatus> SaleStatuses { get; set; }

    public virtual DbSet<SaleType> SaleTypes { get; set; }

    public virtual DbSet<ShippingMethod> ShippingMethods { get; set; }

    public virtual DbSet<Size> Sizes { get; set; }

    public virtual DbSet<Stock> Stocks { get; set; }

    public virtual DbSet<StockVariation> StockVariations { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<TransactionType> TransactionTypes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserDiscount> UserDiscounts { get; set; }

    public virtual DbSet<UserRank> UserRanks { get; set; }

    public virtual DbSet<Variation> Variations { get; set; }

    public virtual DbSet<VariationSingle> VariationSingles { get; set; }

    public virtual DbSet<Wishlist> Wishlists { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=HauLT-LP\\SQLEXPRESS; Database=Unleashed; UID=sa; PWD=root; TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.BrandId).HasName("brand_pkey");

            entity.ToTable("brand");

            entity.Property(e => e.BrandId).HasColumnName("brand_id");
            entity.Property(e => e.BrandCreatedAt).HasColumnName("brand_created_at");
            entity.Property(e => e.BrandDescription)
                .HasMaxLength(255)
                .HasColumnName("brand_description");
            entity.Property(e => e.BrandImageUrl)
                .HasMaxLength(255)
                .HasColumnName("brand_image_url");
            entity.Property(e => e.BrandName)
                .HasMaxLength(255)
                .HasColumnName("brand_name");
            entity.Property(e => e.BrandUpdatedAt).HasColumnName("brand_updated_at");
            entity.Property(e => e.BrandWebsiteUrl)
                .HasMaxLength(255)
                .HasColumnName("brand_website_url");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.VariationId }).HasName("cart_pkey");

            entity.ToTable("cart");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.VariationId).HasColumnName("variation_id");
            entity.Property(e => e.CartQuantity).HasColumnName("cart_quantity");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cart_user_id_fkey");

            entity.HasOne(d => d.Variation).WithMany(p => p.Carts)
                .HasForeignKey(d => d.VariationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("cart_variation_id_fkey");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("category_pkey");

            entity.ToTable("category");

            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CategoryCreatedAt).HasColumnName("category_created_at");
            entity.Property(e => e.CategoryDescription).HasColumnName("category_description");
            entity.Property(e => e.CategoryImageUrl)
                .HasMaxLength(255)
                .HasColumnName("category_image_url");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(255)
                .HasColumnName("category_name");
            entity.Property(e => e.CategoryUpdatedAt).HasColumnName("category_updated_at");
        });

        modelBuilder.Entity<Color>(entity =>
        {
            entity.HasKey(e => e.ColorId).HasName("color_pkey");

            entity.ToTable("color");

            entity.Property(e => e.ColorId).HasColumnName("color_id");
            entity.Property(e => e.ColorHexCode)
                .HasMaxLength(255)
                .HasColumnName("color_hex_code");
            entity.Property(e => e.ColorName)
                .HasMaxLength(255)
                .HasColumnName("color_name");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("comment_pkey");

            entity.ToTable("comment");

            entity.Property(e => e.CommentId).HasColumnName("comment_id");
            entity.Property(e => e.CommentContent)
                .HasMaxLength(255)
                .HasColumnName("comment_content");
            entity.Property(e => e.CommentCreatedAt).HasColumnName("comment_created_at");
            entity.Property(e => e.CommentUpdatedAt).HasColumnName("comment_updated_at");
            entity.Property(e => e.ReviewId).HasColumnName("review_id");

            entity.HasOne(d => d.Review).WithMany(p => p.Comments)
                .HasForeignKey(d => d.ReviewId)
                .HasConstraintName("comment_review_id_fkey");
        });

        modelBuilder.Entity<CommentParent>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("comment_parent");

            entity.Property(e => e.CommentId).HasColumnName("comment_id");
            entity.Property(e => e.CommentParentId).HasColumnName("comment_parent_id");

            entity.HasOne(d => d.Comment).WithMany()
                .HasForeignKey(d => d.CommentId)
                .HasConstraintName("comment_parent_comment_id_fkey");

            entity.HasOne(d => d.CommentParentNavigation).WithMany()
                .HasForeignKey(d => d.CommentParentId)
                .HasConstraintName("comment_parent_comment_parent_id_fkey");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.DiscountId).HasName("discount_pkey");

            entity.ToTable("discount");

            entity.Property(e => e.DiscountId).HasColumnName("discount_id");
            entity.Property(e => e.DiscountCode)
                .HasMaxLength(20)
                .HasColumnName("discount_code");
            entity.Property(e => e.DiscountCreatedAt).HasColumnName("discount_created_at");
            entity.Property(e => e.DiscountDescription)
                .HasMaxLength(255)
                .HasColumnName("discount_description");
            entity.Property(e => e.DiscountEndDate).HasColumnName("discount_end_date");
            entity.Property(e => e.DiscountMaximumValue)
                .HasColumnType("decimal(22, 2)")
                .HasColumnName("discount_maximum_value");
            entity.Property(e => e.DiscountMinimumOrderValue)
                .HasColumnType("decimal(22, 2)")
                .HasColumnName("discount_minimum_order_value");
            entity.Property(e => e.DiscountRankRequirement).HasColumnName("discount_rank_requirement");
            entity.Property(e => e.DiscountStartDate).HasColumnName("discount_start_date");
            entity.Property(e => e.DiscountStatusId).HasColumnName("discount_status_id");
            entity.Property(e => e.DiscountTypeId).HasColumnName("discount_type_id");
            entity.Property(e => e.DiscountUpdatedAt).HasColumnName("discount_updated_at");
            entity.Property(e => e.DiscountUsageCount).HasColumnName("discount_usage_count");
            entity.Property(e => e.DiscountUsageLimit).HasColumnName("discount_usage_limit");
            entity.Property(e => e.DiscountValue)
                .HasColumnType("decimal(22, 2)")
                .HasColumnName("discount_value");

            entity.HasOne(d => d.DiscountRankRequirementNavigation).WithMany(p => p.Discounts)
                .HasForeignKey(d => d.DiscountRankRequirement)
                .HasConstraintName("discount_discount_rank_requirement_fkey");

            entity.HasOne(d => d.DiscountStatus).WithMany(p => p.Discounts)
                .HasForeignKey(d => d.DiscountStatusId)
                .HasConstraintName("discount_discount_status_id_fkey");

            entity.HasOne(d => d.DiscountType).WithMany(p => p.Discounts)
                .HasForeignKey(d => d.DiscountTypeId)
                .HasConstraintName("discount_discount_type_id_fkey");
        });

        modelBuilder.Entity<DiscountStatus>(entity =>
        {
            entity.HasKey(e => e.DiscountStatusId).HasName("discount_status_pkey");

            entity.ToTable("discount_status");

            entity.Property(e => e.DiscountStatusId).HasColumnName("discount_status_id");
            entity.Property(e => e.DiscountStatusName)
                .HasMaxLength(255)
                .HasColumnName("discount_status_name");
        });

        modelBuilder.Entity<DiscountType>(entity =>
        {
            entity.HasKey(e => e.DiscountTypeId).HasName("discount_type_pkey");

            entity.ToTable("discount_type");

            entity.Property(e => e.DiscountTypeId).HasColumnName("discount_type_id");
            entity.Property(e => e.DiscountTypeName)
                .HasMaxLength(255)
                .HasColumnName("discount_type_name");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("notification_pkey");

            entity.ToTable("notification");

            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.IsNotificationDraft).HasColumnName("is_notification_draft");
            entity.Property(e => e.NotificationContent).HasColumnName("notification_content");
            entity.Property(e => e.NotificationCreatedAt).HasColumnName("notification_created_at");
            entity.Property(e => e.NotificationTitle)
                .HasMaxLength(255)
                .HasColumnName("notification_title");
            entity.Property(e => e.NotificationUpdatedAt).HasColumnName("notification_updated_at");
            entity.Property(e => e.UserIdSender).HasColumnName("user_id_sender");

            entity.HasOne(d => d.UserIdSenderNavigation).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserIdSender)
                .HasConstraintName("notification_user_id_sender_fkey");
        });

        modelBuilder.Entity<NotificationUser>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("notification_user");

            entity.Property(e => e.IsNotificationDeleted).HasColumnName("is_notification_deleted");
            entity.Property(e => e.IsNotificationViewed).HasColumnName("is_notification_viewed");
            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Notification).WithMany()
                .HasForeignKey(d => d.NotificationId)
                .HasConstraintName("notification_user_notification_id_fkey");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("notification_user_user_id_fkey");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("order_pkey");

            entity.ToTable("order");

            entity.Property(e => e.OrderId)
                .HasMaxLength(255)
                .HasColumnName("order_id");
            entity.Property(e => e.DiscountId).HasColumnName("discount_id");
            entity.Property(e => e.InchargeEmployeeId).HasColumnName("incharge_employee_id");
            entity.Property(e => e.OrderBillingAddress)
                .HasMaxLength(255)
                .HasColumnName("order_billing_address");
            entity.Property(e => e.OrderCreatedAt).HasColumnName("order_created_at");
            entity.Property(e => e.OrderDate).HasColumnName("order_date");
            entity.Property(e => e.OrderExpectedDeliveryDate).HasColumnName("order_expected_delivery_date");
            entity.Property(e => e.OrderNote).HasColumnName("order_note");
            entity.Property(e => e.OrderStatusId).HasColumnName("order_status_id");
            entity.Property(e => e.OrderTax)
                .HasColumnType("numeric(3, 2)")
                .HasColumnName("order_tax");
            entity.Property(e => e.OrderTotalAmount)
                .HasColumnType("decimal(22, 2)")
                .HasColumnName("order_total_amount");
            entity.Property(e => e.OrderTrackingNumber)
                .HasMaxLength(50)
                .HasColumnName("order_tracking_number");
            entity.Property(e => e.OrderTransactionReference)
                .HasMaxLength(255)
                .HasColumnName("order_transaction_reference");
            entity.Property(e => e.OrderUpdatedAt).HasColumnName("order_updated_at");
            entity.Property(e => e.PaymentMethodId).HasColumnName("payment_method_id");
            entity.Property(e => e.ShippingMethodId).HasColumnName("shipping_method_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Discount).WithMany(p => p.Orders)
                .HasForeignKey(d => d.DiscountId)
                .HasConstraintName("order_discount_id_fkey");

            entity.HasOne(d => d.InchargeEmployee).WithMany(p => p.OrderInchargeEmployees)
                .HasForeignKey(d => d.InchargeEmployeeId)
                .HasConstraintName("order_incharge_employee_id_fkey");

            entity.HasOne(d => d.OrderStatus).WithMany(p => p.Orders)
                .HasForeignKey(d => d.OrderStatusId)
                .HasConstraintName("order_order_status_id_fkey");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PaymentMethodId)
                .HasConstraintName("order_payment_method_id_fkey");

            entity.HasOne(d => d.ShippingMethod).WithMany(p => p.Orders)
                .HasForeignKey(d => d.ShippingMethodId)
                .HasConstraintName("order_shipping_method_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.OrderUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("order_user_id_fkey");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.OrderStatusId).HasName("order_status_pkey");

            entity.ToTable("order_status");

            entity.Property(e => e.OrderStatusId).HasColumnName("order_status_id");
            entity.Property(e => e.OrderStatusName)
                .HasMaxLength(255)
                .HasColumnName("order_status_name");
        });

        modelBuilder.Entity<OrderVariationSingle>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.VariationSingleId }).HasName("order_variation_single_pkey");

            entity.ToTable("order_variation_single");

            entity.Property(e => e.OrderId)
                .HasMaxLength(255)
                .HasColumnName("order_id");
            entity.Property(e => e.VariationSingleId).HasColumnName("variation_single_id");
            entity.Property(e => e.SaleId).HasColumnName("sale_id");
            entity.Property(e => e.VariationPriceAtPurchase)
                .HasColumnType("decimal(22, 2)")
                .HasColumnName("variation_price_at_purchase");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderVariationSingles)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_variation_single_order_id_fkey");

            entity.HasOne(d => d.VariationSingle).WithMany(p => p.OrderVariationSingles)
                .HasForeignKey(d => d.VariationSingleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_variation_single_variation_single_id_fkey");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.PaymentMethodId).HasName("payment_method_pkey");

            entity.ToTable("payment_method");

            entity.Property(e => e.PaymentMethodId).HasColumnName("payment_method_id");
            entity.Property(e => e.PaymentMethodName)
                .HasMaxLength(255)
                .HasColumnName("payment_method_name");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("product_pkey");

            entity.ToTable("product");

            entity.Property(e => e.ProductId)
                .ValueGeneratedNever()
                .HasColumnName("product_id");
            entity.Property(e => e.BrandId).HasColumnName("brand_id");
            entity.Property(e => e.ProductCode)
                .HasMaxLength(255)
                .HasColumnName("product_code");
            entity.Property(e => e.ProductCreatedAt).HasColumnName("product_created_at");
            entity.Property(e => e.ProductDescription).HasColumnName("product_description");
            entity.Property(e => e.ProductName)
                .HasMaxLength(255)
                .HasColumnName("product_name");
            entity.Property(e => e.ProductStatusId).HasColumnName("product_status_id");
            entity.Property(e => e.ProductUpdatedAt).HasColumnName("product_updated_at");

            entity.HasOne(d => d.Brand).WithMany(p => p.Products)
                .HasForeignKey(d => d.BrandId)
                .HasConstraintName("product_brand_id_fkey");

            entity.HasOne(d => d.ProductStatus).WithMany(p => p.Products)
                .HasForeignKey(d => d.ProductStatusId)
                .HasConstraintName("product_product_status_id_fkey");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("product_category");

            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");

            entity.HasOne(d => d.Category).WithMany()
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("product_category_category_id_fkey");

            entity.HasOne(d => d.Product).WithMany()
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("product_category_product_id_fkey");
        });

        modelBuilder.Entity<ProductStatus>(entity =>
        {
            entity.HasKey(e => e.ProductStatusId).HasName("product_status_pkey");

            entity.ToTable("product_status");

            entity.Property(e => e.ProductStatusId).HasColumnName("product_status_id");
            entity.Property(e => e.ProductStatusName)
                .HasMaxLength(255)
                .HasColumnName("product_status_name");
        });

        modelBuilder.Entity<Provider>(entity =>
        {
            entity.HasKey(e => e.ProviderId).HasName("provider_pkey");

            entity.ToTable("provider");

            entity.Property(e => e.ProviderId).HasColumnName("provider_id");
            entity.Property(e => e.ProviderAddress)
                .HasMaxLength(255)
                .HasColumnName("provider_address");
            entity.Property(e => e.ProviderCreatedAt).HasColumnName("provider_created_at");
            entity.Property(e => e.ProviderEmail)
                .HasMaxLength(255)
                .HasColumnName("provider_email");
            entity.Property(e => e.ProviderImageUrl)
                .HasMaxLength(255)
                .HasColumnName("provider_image_url");
            entity.Property(e => e.ProviderName)
                .HasMaxLength(255)
                .HasColumnName("provider_name");
            entity.Property(e => e.ProviderPhone)
                .HasMaxLength(12)
                .HasColumnName("provider_phone");
            entity.Property(e => e.ProviderUpdatedAt).HasColumnName("provider_updated_at");
        });

        modelBuilder.Entity<Rank>(entity =>
        {
            entity.HasKey(e => e.RankId).HasName("rank_pkey");

            entity.ToTable("rank");

            entity.Property(e => e.RankId).HasColumnName("rank_id");
            entity.Property(e => e.RankBaseDiscount)
                .HasColumnType("numeric(3, 2)")
                .HasColumnName("rank_base_discount");
            entity.Property(e => e.RankName).HasColumnName("rank_name");
            entity.Property(e => e.RankNum).HasColumnName("rank_num");
            entity.Property(e => e.RankPaymentRequirement)
                .HasColumnType("decimal(22, 2)")
                .HasColumnName("rank_payment_requirement");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("review_pkey");

            entity.ToTable("review");

            entity.Property(e => e.ReviewId).HasColumnName("review_id");
            entity.Property(e => e.OrderId)
                .HasMaxLength(255)
                .HasColumnName("order_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ReviewRating).HasColumnName("review_rating");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Order).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("review_order_id_fkey");

            entity.HasOne(d => d.Product).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("review_product_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("review_user_id_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("role_pkey");

            entity.ToTable("role");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(255)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(e => e.SaleId).HasName("sale_pkey");

            entity.ToTable("sale");

            entity.Property(e => e.SaleId).HasColumnName("sale_id");
            entity.Property(e => e.SaleCreatedAt).HasColumnName("sale_created_at");
            entity.Property(e => e.SaleEndDate).HasColumnName("sale_end_date");
            entity.Property(e => e.SaleStartDate).HasColumnName("sale_start_date");
            entity.Property(e => e.SaleStatusId).HasColumnName("sale_status_id");
            entity.Property(e => e.SaleTypeId).HasColumnName("sale_type_id");
            entity.Property(e => e.SaleUpdatedAt).HasColumnName("sale_updated_at");
            entity.Property(e => e.SaleValue)
                .HasColumnType("decimal(22, 2)")
                .HasColumnName("sale_value");

            entity.HasOne(d => d.SaleStatus).WithMany(p => p.Sales)
                .HasForeignKey(d => d.SaleStatusId)
                .HasConstraintName("sale_sale_status_id_fkey");

            entity.HasOne(d => d.SaleType).WithMany(p => p.Sales)
                .HasForeignKey(d => d.SaleTypeId)
                .HasConstraintName("sale_sale_type_id_fkey");

            entity.HasMany(d => d.Products).WithMany(p => p.Sales)
                .UsingEntity<Dictionary<string, object>>(
                    "SaleProduct",
                    r => r.HasOne<Product>().WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("sale_product_product_id_fkey"),
                    l => l.HasOne<Sale>().WithMany()
                        .HasForeignKey("SaleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("sale_product_sale_id_fkey"),
                    j =>
                    {
                        j.HasKey("SaleId", "ProductId").HasName("sale_product_pkey");
                        j.ToTable("sale_product");
                        j.IndexerProperty<int>("SaleId").HasColumnName("sale_id");
                        j.IndexerProperty<Guid>("ProductId").HasColumnName("product_id");
                    });
        });

        modelBuilder.Entity<SaleStatus>(entity =>
        {
            entity.HasKey(e => e.SaleStatusId).HasName("sale_status_pkey");

            entity.ToTable("sale_status");

            entity.Property(e => e.SaleStatusId).HasColumnName("sale_status_id");
            entity.Property(e => e.SaleStatusName)
                .HasMaxLength(255)
                .HasColumnName("sale_status_name");
        });

        modelBuilder.Entity<SaleType>(entity =>
        {
            entity.HasKey(e => e.SaleTypeId).HasName("sale_type_pkey");

            entity.ToTable("sale_type");

            entity.Property(e => e.SaleTypeId).HasColumnName("sale_type_id");
            entity.Property(e => e.SaleTypeName)
                .HasMaxLength(255)
                .HasColumnName("sale_type_name");
        });

        modelBuilder.Entity<ShippingMethod>(entity =>
        {
            entity.HasKey(e => e.ShippingMethodId).HasName("shipping_method_pkey");

            entity.ToTable("shipping_method");

            entity.Property(e => e.ShippingMethodId).HasColumnName("shipping_method_id");
            entity.Property(e => e.ShippingMethodName)
                .HasMaxLength(255)
                .HasColumnName("shipping_method_name");
        });

        modelBuilder.Entity<Size>(entity =>
        {
            entity.HasKey(e => e.SizeId).HasName("size_pkey");

            entity.ToTable("size");

            entity.Property(e => e.SizeId).HasColumnName("size_id");
            entity.Property(e => e.SizeName)
                .HasMaxLength(255)
                .HasColumnName("size_name");
        });

        modelBuilder.Entity<Stock>(entity =>
        {
            entity.HasKey(e => e.StockId).HasName("stock_pkey");

            entity.ToTable("stock");

            entity.Property(e => e.StockId).HasColumnName("stock_id");
            entity.Property(e => e.StockAddress)
                .HasMaxLength(255)
                .HasColumnName("stock_address");
            entity.Property(e => e.StockName)
                .HasMaxLength(255)
                .HasColumnName("stock_name");
        });

        modelBuilder.Entity<StockVariation>(entity =>
        {
            entity.HasKey(e => new { e.StockId, e.VariationId }).HasName("stock_variation_pkey");

            entity.ToTable("stock_variation");

            entity.Property(e => e.StockId).HasColumnName("stock_id");
            entity.Property(e => e.VariationId).HasColumnName("variation_id");
            entity.Property(e => e.StockQuantity).HasColumnName("stock_quantity");

            entity.HasOne(d => d.Stock).WithMany(p => p.StockVariations)
                .HasForeignKey(d => d.StockId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("stock_variation_stock_id_fkey");

            entity.HasOne(d => d.Variation).WithMany(p => p.StockVariations)
                .HasForeignKey(d => d.VariationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("stock_variation_variation_id_fkey");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("transaction_pkey");

            entity.ToTable("transaction");

            entity.Property(e => e.TransactionId).HasColumnName("transaction_id");
            entity.Property(e => e.InchargeEmployeeId).HasColumnName("incharge_employee_id");
            entity.Property(e => e.ProviderId).HasColumnName("provider_id");
            entity.Property(e => e.StockId).HasColumnName("stock_id");
            entity.Property(e => e.TransactionDate)
                .HasPrecision(6)
                .HasColumnName("transaction_date");
            entity.Property(e => e.TransactionProductPrice)
                .HasColumnType("decimal(22, 2)")
                .HasColumnName("transaction_product_price");
            entity.Property(e => e.TransactionQuantity).HasColumnName("transaction_quantity");
            entity.Property(e => e.TransactionTypeId).HasColumnName("transaction_type_id");
            entity.Property(e => e.VariationId).HasColumnName("variation_id");

            entity.HasOne(d => d.InchargeEmployee).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.InchargeEmployeeId)
                .HasConstraintName("transaction_incharge_employee_id_fkey");

            entity.HasOne(d => d.Provider).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.ProviderId)
                .HasConstraintName("transaction_provider_id_fkey");

            entity.HasOne(d => d.Stock).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.StockId)
                .HasConstraintName("transaction_stock_id_fkey");

            entity.HasOne(d => d.TransactionType).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.TransactionTypeId)
                .HasConstraintName("transaction_transaction_type_id_fkey");

            entity.HasOne(d => d.Variation).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.VariationId)
                .HasConstraintName("transaction_variation_id_fkey");
        });

        modelBuilder.Entity<TransactionType>(entity =>
        {
            entity.HasKey(e => e.TransactionTypeId).HasName("transaction_type_pkey");

            entity.ToTable("transaction_type");

            entity.Property(e => e.TransactionTypeId).HasColumnName("transaction_type_id");
            entity.Property(e => e.TransactionTypeName)
                .HasMaxLength(255)
                .HasColumnName("transaction_type_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("user_pkey");

            entity.ToTable("user");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.IsUserEnabled).HasColumnName("is_user_enabled");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UserAddress)
                .HasMaxLength(255)
                .HasColumnName("user_address");
            entity.Property(e => e.UserBirthdate)
                .HasMaxLength(255)
                .HasColumnName("user_birthdate");
            entity.Property(e => e.UserCreatedAt).HasColumnName("user_created_at");
            entity.Property(e => e.UserCurrentPaymentMethod)
                .HasMaxLength(255)
                .HasColumnName("user_current_payment_method");
            entity.Property(e => e.UserEmail)
                .HasMaxLength(255)
                .HasColumnName("user_email");
            entity.Property(e => e.UserFullname)
                .HasMaxLength(255)
                .HasColumnName("user_fullname");
            entity.Property(e => e.UserGoogleId)
                .HasMaxLength(255)
                .HasColumnName("user_google_id");
            entity.Property(e => e.UserImage)
                .HasMaxLength(255)
                .HasColumnName("user_image");
            entity.Property(e => e.UserPassword)
                .HasMaxLength(255)
                .HasColumnName("user_password");
            entity.Property(e => e.UserPhone)
                .HasMaxLength(12)
                .HasColumnName("user_phone");
            entity.Property(e => e.UserUpdatedAt).HasColumnName("user_updated_at");
            entity.Property(e => e.UserUsername)
                .HasMaxLength(255)
                .HasColumnName("user_username");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("user_role_id_fkey");
        });

        modelBuilder.Entity<UserDiscount>(entity =>
        {
            entity.HasKey(e => new { e.DiscountId, e.UserId }).HasName("user_discount_pkey");

            entity.ToTable("user_discount");

            entity.Property(e => e.DiscountId).HasColumnName("discount_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.DiscountUsedAt).HasColumnName("discount_used_at");
            entity.Property(e => e.IsDiscountUsed).HasColumnName("is_discount_used");

            entity.HasOne(d => d.Discount).WithMany(p => p.UserDiscounts)
                .HasForeignKey(d => d.DiscountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_discount_discount_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.UserDiscounts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_discount_user_id_fkey");
        });

        modelBuilder.Entity<UserRank>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("user_rank_pkey");

            entity.ToTable("user_rank");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.MoneySpent)
                .HasColumnType("decimal(22, 2)")
                .HasColumnName("money_spent");
            entity.Property(e => e.RankCreatedDate).HasColumnName("rank_created_date");
            entity.Property(e => e.RankExpireDate).HasColumnName("rank_expire_date");
            entity.Property(e => e.RankId).HasColumnName("rank_id");
            entity.Property(e => e.RankStatus).HasColumnName("rank_status");
            entity.Property(e => e.RankUpdatedDate).HasColumnName("rank_updated_date");

            entity.HasOne(d => d.Rank).WithMany(p => p.UserRanks)
                .HasForeignKey(d => d.RankId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_rank_rank_id_fkey");

            entity.HasOne(d => d.User).WithOne(p => p.UserRank)
                .HasForeignKey<UserRank>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_rank_user_id_fkey");
        });

        modelBuilder.Entity<Variation>(entity =>
        {
            entity.HasKey(e => e.VariationId).HasName("PK_variation_temp");

            entity.ToTable("variation");

            entity.Property(e => e.VariationId).HasColumnName("variation_id");
            entity.Property(e => e.ColorId).HasColumnName("color_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.SizeId).HasColumnName("size_id");
            entity.Property(e => e.VariationImage).HasColumnName("variation_image");
            entity.Property(e => e.VariationPrice)
                .HasColumnType("decimal(22, 2)")
                .HasColumnName("variation_price");

            entity.HasOne(d => d.Color).WithMany(p => p.Variations)
                .HasForeignKey(d => d.ColorId)
                .HasConstraintName("FK9gqn7oby75ixq0fg8w902pjcf");

            entity.HasOne(d => d.Product).WithMany(p => p.Variations)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK1hxfv06p366bhb8sce1djt2v7");

            entity.HasOne(d => d.Size).WithMany(p => p.Variations)
                .HasForeignKey(d => d.SizeId)
                .HasConstraintName("FKe3asl55h6omj6x27479hnprov");
        });

        modelBuilder.Entity<VariationSingle>(entity =>
        {
            entity.HasKey(e => e.VariationSingleId).HasName("PK_variation_single_temp");

            entity.ToTable("variation_single");

            entity.Property(e => e.VariationSingleId).HasColumnName("variation_single_id");
            entity.Property(e => e.IsVariationSingleBought).HasColumnName("is_variation_single_bought");
            entity.Property(e => e.VariationId).HasColumnName("variation_id");
            entity.Property(e => e.VariationSingleCode)
                .HasMaxLength(255)
                .HasColumnName("variation_single_code");

            entity.HasOne(d => d.Variation).WithMany(p => p.VariationSingles)
                .HasForeignKey(d => d.VariationId)
                .HasConstraintName("FKpo4u72nmt63l6xr398m2wovvb");
        });

        modelBuilder.Entity<Wishlist>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("wishlist");

            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Product).WithMany()
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("wishlist_product_id_fkey");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("wishlist_user_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
