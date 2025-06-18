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

    public virtual DbSet<combo> combos { get; set; }

    public virtual DbSet<combo_product> combo_products { get; set; }

    public virtual DbSet<daily_revenue> daily_revenues { get; set; }

    public virtual DbSet<order> orders { get; set; }

    public virtual DbSet<order_combo> order_combos { get; set; }

    public virtual DbSet<order_item> order_items { get; set; }

    public virtual DbSet<order_item_topping> order_item_toppings { get; set; }

    public virtual DbSet<product> products { get; set; }

    public virtual DbSet<topping> toppings { get; set; }

    public virtual DbSet<user> users { get; set; }

   
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<combo>(entity =>
        {
            entity.HasKey(e => e.combo_id).HasName("PRIMARY");

            entity.Property(e => e.combo_name).HasMaxLength(100);
            entity.Property(e => e.combo_price).HasPrecision(10, 2);
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.description).HasColumnType("text");
            entity.Property(e => e.image_url).HasMaxLength(255);
            entity.Property(e => e.is_available).HasDefaultValueSql("'1'");
            entity.Property(e => e.updated_at)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
        });

        modelBuilder.Entity<combo_product>(entity =>
        {
            entity.HasKey(e => e.combo_product_id).HasName("PRIMARY");

            entity.HasIndex(e => e.combo_id, "combo_id");

            entity.HasIndex(e => e.product_id, "product_id");

            entity.Property(e => e.quantity).HasDefaultValueSql("'1'");

            entity.HasOne(d => d.combo).WithMany(p => p.combo_products)
                .HasForeignKey(d => d.combo_id)
                .HasConstraintName("combo_products_ibfk_1");

            entity.HasOne(d => d.product).WithMany(p => p.combo_products)
                .HasForeignKey(d => d.product_id)
                .HasConstraintName("combo_products_ibfk_2");
        });

        modelBuilder.Entity<daily_revenue>(entity =>
        {
            entity.HasKey(e => e.revenue_id).HasName("PRIMARY");

            entity.ToTable("daily_revenue");

            entity.HasIndex(e => e.revenue_date, "revenue_date").IsUnique();

            entity.Property(e => e.card_revenue)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("'0.00'");
            entity.Property(e => e.cash_revenue)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("'0.00'");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.digital_wallet_revenue)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("'0.00'");
            entity.Property(e => e.total_orders).HasDefaultValueSql("'0'");
            entity.Property(e => e.total_revenue)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("'0.00'");
            entity.Property(e => e.updated_at)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
        });

        modelBuilder.Entity<order>(entity =>
        {
            entity.HasKey(e => e.order_id).HasName("PRIMARY");

            entity.HasIndex(e => e.user_id, "user_id");

            entity.Property(e => e.completed_at).HasColumnType("timestamp");
            entity.Property(e => e.confirmed_at).HasColumnType("timestamp");
            entity.Property(e => e.delivery_address).HasColumnType("text");
            entity.Property(e => e.notes).HasColumnType("text");
            entity.Property(e => e.order_date)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.order_status)
                .HasDefaultValueSql("'pending'")
                .HasColumnType("enum('pending','confirmed','preparing','ready','delivered','cancelled')");
            entity.Property(e => e.payment_method)
                .HasDefaultValueSql("'cash'")
                .HasColumnType("enum('cash','card','digital_wallet')");
            entity.Property(e => e.payment_status)
                .HasDefaultValueSql("'pending'")
                .HasColumnType("enum('pending','paid','failed')");
            entity.Property(e => e.total_amount).HasPrecision(10, 2);

            entity.HasOne(d => d.user).WithMany(p => p.orders)
                .HasForeignKey(d => d.user_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("orders_ibfk_1");
        });

        modelBuilder.Entity<order_combo>(entity =>
        {
            entity.HasKey(e => e.order_combo_id).HasName("PRIMARY");

            entity.HasIndex(e => e.combo_id, "combo_id");

            entity.HasIndex(e => e.order_id, "order_id");

            entity.Property(e => e.quantity).HasDefaultValueSql("'1'");
            entity.Property(e => e.subtotal).HasPrecision(10, 2);
            entity.Property(e => e.unit_price).HasPrecision(10, 2);

            entity.HasOne(d => d.combo).WithMany(p => p.order_combos)
                .HasForeignKey(d => d.combo_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_combos_ibfk_2");

            entity.HasOne(d => d.order).WithMany(p => p.order_combos)
                .HasForeignKey(d => d.order_id)
                .HasConstraintName("order_combos_ibfk_1");
        });

        modelBuilder.Entity<order_item>(entity =>
        {
            entity.HasKey(e => e.order_item_id).HasName("PRIMARY");

            entity.HasIndex(e => e.order_id, "order_id");

            entity.HasIndex(e => e.product_id, "product_id");

            entity.Property(e => e.quantity).HasDefaultValueSql("'1'");
            entity.Property(e => e.subtotal).HasPrecision(10, 2);
            entity.Property(e => e.unit_price).HasPrecision(10, 2);

            entity.HasOne(d => d.order).WithMany(p => p.order_items)
                .HasForeignKey(d => d.order_id)
                .HasConstraintName("order_items_ibfk_1");

            entity.HasOne(d => d.product).WithMany(p => p.order_items)
                .HasForeignKey(d => d.product_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_items_ibfk_2");
        });

        modelBuilder.Entity<order_item_topping>(entity =>
        {
            entity.HasKey(e => e.order_item_topping_id).HasName("PRIMARY");

            entity.HasIndex(e => e.order_item_id, "order_item_id");

            entity.HasIndex(e => e.topping_id, "topping_id");

            entity.Property(e => e.quantity).HasDefaultValueSql("'1'");
            entity.Property(e => e.subtotal).HasPrecision(10, 2);
            entity.Property(e => e.unit_price).HasPrecision(10, 2);

            entity.HasOne(d => d.order_item).WithMany(p => p.order_item_toppings)
                .HasForeignKey(d => d.order_item_id)
                .HasConstraintName("order_item_toppings_ibfk_1");

            entity.HasOne(d => d.topping).WithMany(p => p.order_item_toppings)
                .HasForeignKey(d => d.topping_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_item_toppings_ibfk_2");
        });

        modelBuilder.Entity<product>(entity =>
        {
            entity.HasKey(e => e.product_id).HasName("PRIMARY");

            entity.Property(e => e.base_price).HasPrecision(10, 2);
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.description).HasColumnType("text");
            entity.Property(e => e.image_url).HasMaxLength(255);
            entity.Property(e => e.is_available).HasDefaultValueSql("'1'");
            entity.Property(e => e.product_name).HasMaxLength(100);
            entity.Property(e => e.spice_level)
                .HasDefaultValueSql("'medium'")
                .HasColumnType("enum('mild','medium','hot','extra_hot')");
            entity.Property(e => e.updated_at)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
        });

        modelBuilder.Entity<topping>(entity =>
        {
            entity.HasKey(e => e.topping_id).HasName("PRIMARY");

            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.description).HasColumnType("text");
            entity.Property(e => e.is_available).HasDefaultValueSql("'1'");
            entity.Property(e => e.price).HasPrecision(10, 2);
            entity.Property(e => e.topping_name).HasMaxLength(50);
            entity.Property(e => e.updated_at)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
        });

        modelBuilder.Entity<user>(entity =>
        {
            entity.HasKey(e => e.user_id).HasName("PRIMARY");

            entity.HasIndex(e => e.email, "email").IsUnique();

            entity.HasIndex(e => e.username, "username").IsUnique();

            entity.Property(e => e.address).HasColumnType("text");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.email).HasMaxLength(100);
            entity.Property(e => e.full_name).HasMaxLength(100);
            entity.Property(e => e.is_active).HasDefaultValueSql("'1'");
            entity.Property(e => e.password).HasMaxLength(255);
            entity.Property(e => e.phone).HasMaxLength(15);
            entity.Property(e => e.role)
                .HasDefaultValueSql("'customer'")
                .HasColumnType("enum('customer','staff','admin')");
            entity.Property(e => e.updated_at)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.username).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
