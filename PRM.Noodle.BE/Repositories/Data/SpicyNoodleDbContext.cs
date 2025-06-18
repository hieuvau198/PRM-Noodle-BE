using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using Repositories.Models;

namespace Repositories.Data;

public partial class SpicyNoodleDbContext : DbContext
{
    public SpicyNoodleDbContext()
    {
    }

    public SpicyNoodleDbContext(DbContextOptions<SpicyNoodleDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Combo> Combos { get; set; }

    public virtual DbSet<ComboProduct> ComboProducts { get; set; }

    public virtual DbSet<DailyRevenue> DailyRevenues { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderCombo> OrderCombos { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<OrderItemTopping> OrderItemToppings { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Topping> Toppings { get; set; }

    public virtual DbSet<User> Users { get; set; }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Combo>(entity =>
        {
            entity.HasKey(e => e.ComboId).HasName("PRIMARY");

            entity.ToTable("combos");

            entity.Property(e => e.ComboId).HasColumnName("combo_id");
            entity.Property(e => e.ComboName)
                .HasMaxLength(100)
                .HasColumnName("combo_name");
            entity.Property(e => e.ComboPrice)
                .HasPrecision(10, 2)
                .HasColumnName("combo_price");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("image_url");
            entity.Property(e => e.IsAvailable)
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_available");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<ComboProduct>(entity =>
        {
            entity.HasKey(e => e.ComboProductId).HasName("PRIMARY");

            entity.ToTable("combo_products");

            entity.HasIndex(e => e.ComboId, "combo_id");

            entity.HasIndex(e => e.ProductId, "product_id");

            entity.Property(e => e.ComboProductId).HasColumnName("combo_product_id");
            entity.Property(e => e.ComboId).HasColumnName("combo_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity)
                .HasDefaultValueSql("'1'")
                .HasColumnName("quantity");

            entity.HasOne(d => d.Combo).WithMany(p => p.ComboProducts)
                .HasForeignKey(d => d.ComboId)
                .HasConstraintName("combo_products_ibfk_1");

            entity.HasOne(d => d.Product).WithMany(p => p.ComboProducts)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("combo_products_ibfk_2");
        });

        modelBuilder.Entity<DailyRevenue>(entity =>
        {
            entity.HasKey(e => e.RevenueId).HasName("PRIMARY");

            entity.ToTable("daily_revenue");

            entity.HasIndex(e => e.RevenueDate, "revenue_date").IsUnique();

            entity.Property(e => e.RevenueId).HasColumnName("revenue_id");
            entity.Property(e => e.CardRevenue)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("'0.00'")
                .HasColumnName("card_revenue");
            entity.Property(e => e.CashRevenue)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("'0.00'")
                .HasColumnName("cash_revenue");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.DigitalWalletRevenue)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("'0.00'")
                .HasColumnName("digital_wallet_revenue");
            entity.Property(e => e.RevenueDate).HasColumnName("revenue_date");
            entity.Property(e => e.TotalOrders)
                .HasDefaultValueSql("'0'")
                .HasColumnName("total_orders");
            entity.Property(e => e.TotalRevenue)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("'0.00'")
                .HasColumnName("total_revenue");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PRIMARY");

            entity.ToTable("orders");

            entity.HasIndex(e => e.UserId, "user_id");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.CompletedAt)
                .HasColumnType("timestamp")
                .HasColumnName("completed_at");
            entity.Property(e => e.ConfirmedAt)
                .HasColumnType("timestamp")
                .HasColumnName("confirmed_at");
            entity.Property(e => e.DeliveryAddress)
                .HasColumnType("text")
                .HasColumnName("delivery_address");
            entity.Property(e => e.Notes)
                .HasColumnType("text")
                .HasColumnName("notes");
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("order_date");
            entity.Property(e => e.OrderStatus)
                .HasDefaultValueSql("'pending'")
                .HasColumnType("enum('pending','confirmed','preparing','ready','delivered','cancelled')")
                .HasColumnName("order_status");
            entity.Property(e => e.PaymentMethod)
                .HasDefaultValueSql("'cash'")
                .HasColumnType("enum('cash','card','digital_wallet')")
                .HasColumnName("payment_method");
            entity.Property(e => e.PaymentStatus)
                .HasDefaultValueSql("'pending'")
                .HasColumnType("enum('pending','paid','failed')")
                .HasColumnName("payment_status");
            entity.Property(e => e.TotalAmount)
                .HasPrecision(10, 2)
                .HasColumnName("total_amount");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("orders_ibfk_1");
        });

        modelBuilder.Entity<OrderCombo>(entity =>
        {
            entity.HasKey(e => e.OrderComboId).HasName("PRIMARY");

            entity.ToTable("order_combos");

            entity.HasIndex(e => e.ComboId, "combo_id");

            entity.HasIndex(e => e.OrderId, "order_id");

            entity.Property(e => e.OrderComboId).HasColumnName("order_combo_id");
            entity.Property(e => e.ComboId).HasColumnName("combo_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Quantity)
                .HasDefaultValueSql("'1'")
                .HasColumnName("quantity");
            entity.Property(e => e.Subtotal)
                .HasPrecision(10, 2)
                .HasColumnName("subtotal");
            entity.Property(e => e.UnitPrice)
                .HasPrecision(10, 2)
                .HasColumnName("unit_price");

            entity.HasOne(d => d.Combo).WithMany(p => p.OrderCombos)
                .HasForeignKey(d => d.ComboId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_combos_ibfk_2");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderCombos)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("order_combos_ibfk_1");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId).HasName("PRIMARY");

            entity.ToTable("order_items");

            entity.HasIndex(e => e.OrderId, "order_id");

            entity.HasIndex(e => e.ProductId, "product_id");

            entity.Property(e => e.OrderItemId).HasColumnName("order_item_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity)
                .HasDefaultValueSql("'1'")
                .HasColumnName("quantity");
            entity.Property(e => e.Subtotal)
                .HasPrecision(10, 2)
                .HasColumnName("subtotal");
            entity.Property(e => e.UnitPrice)
                .HasPrecision(10, 2)
                .HasColumnName("unit_price");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("order_items_ibfk_1");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_items_ibfk_2");
        });

        modelBuilder.Entity<OrderItemTopping>(entity =>
        {
            entity.HasKey(e => e.OrderItemToppingId).HasName("PRIMARY");

            entity.ToTable("order_item_toppings");

            entity.HasIndex(e => e.OrderItemId, "order_item_id");

            entity.HasIndex(e => e.ToppingId, "topping_id");

            entity.Property(e => e.OrderItemToppingId).HasColumnName("order_item_topping_id");
            entity.Property(e => e.OrderItemId).HasColumnName("order_item_id");
            entity.Property(e => e.Quantity)
                .HasDefaultValueSql("'1'")
                .HasColumnName("quantity");
            entity.Property(e => e.Subtotal)
                .HasPrecision(10, 2)
                .HasColumnName("subtotal");
            entity.Property(e => e.ToppingId).HasColumnName("topping_id");
            entity.Property(e => e.UnitPrice)
                .HasPrecision(10, 2)
                .HasColumnName("unit_price");

            entity.HasOne(d => d.OrderItem).WithMany(p => p.OrderItemToppings)
                .HasForeignKey(d => d.OrderItemId)
                .HasConstraintName("order_item_toppings_ibfk_1");

            entity.HasOne(d => d.Topping).WithMany(p => p.OrderItemToppings)
                .HasForeignKey(d => d.ToppingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_item_toppings_ibfk_2");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PRIMARY");

            entity.ToTable("products");

            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.BasePrice)
                .HasPrecision(10, 2)
                .HasColumnName("base_price");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("image_url");
            entity.Property(e => e.IsAvailable)
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_available");
            entity.Property(e => e.ProductName)
                .HasMaxLength(100)
                .HasColumnName("product_name");
            entity.Property(e => e.SpiceLevel)
                .HasDefaultValueSql("'medium'")
                .HasColumnType("enum('mild','medium','hot','extra_hot')")
                .HasColumnName("spice_level");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Topping>(entity =>
        {
            entity.HasKey(e => e.ToppingId).HasName("PRIMARY");

            entity.ToTable("toppings");

            entity.Property(e => e.ToppingId).HasColumnName("topping_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.IsAvailable)
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_available");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
            entity.Property(e => e.ToppingName)
                .HasMaxLength(50)
                .HasColumnName("topping_name");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "email").IsUnique();

            entity.HasIndex(e => e.Username, "username").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Address)
                .HasColumnType("text")
                .HasColumnName("address");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_active");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .HasColumnName("phone");
            entity.Property(e => e.Role)
                .HasDefaultValueSql("'customer'")
                .HasColumnType("enum('customer','staff','admin')")
                .HasColumnName("role");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
