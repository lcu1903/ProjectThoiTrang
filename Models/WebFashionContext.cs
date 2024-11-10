using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ProjectThoiTrang.Models;

public partial class WebFashionContext : DbContext
{
    public WebFashionContext()
    {
    }

    public WebFashionContext(DbContextOptions<WebFashionContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartDetail> CartDetails { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-0RDPJVL\\MSSQLSERVER1;Initial Catalog=WebFashion;Integrated Security=True;User ID=sa;Password=Anhdasai123@;Trust Server Certificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Email);

            entity.ToTable("Account");

            entity.Property(e => e.Email).HasMaxLength(250);

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_Account_Role");
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.ToTable("Brand");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK_Order");

            entity.ToTable("Cart");

            entity.HasOne(d => d.Cus).WithMany(p => p.Carts)
                .HasForeignKey(d => d.CusId)
                .HasConstraintName("FK_Order_Customer");
        });

        modelBuilder.Entity<CartDetail>(entity =>
        {
            entity.HasKey(e => new { e.CartId, e.ProductId }).HasName("PK_OrderDetail");

            entity.ToTable("CartDetail");

            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartDetails)
                .HasForeignKey(d => d.CartId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetail_Order1");

            entity.HasOne(d => d.Product).WithMany(p => p.CartDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetail_Product1");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CatId);

            entity.ToTable("Category");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CusId);

            entity.ToTable("Customer");

            entity.Property(e => e.CusId).ValueGeneratedNever();
            entity.Property(e => e.Email).HasMaxLength(250);

            entity.HasOne(d => d.EmailNavigation).WithMany(p => p.Customers)
                .HasForeignKey(d => d.Email)
                .HasConstraintName("FK_Customer_Account");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BCF8A47A5D5");

            entity.Property(e => e.OrderId).HasMaxLength(50);
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.TransactionStatus).HasMaxLength(50);

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_Orders_Customers");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.ProductId });

            entity.Property(e => e.OrderId).HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetails_Orders1");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetails_Product");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");

            entity.Property(e => e.ShortDesc)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Brand).WithMany(p => p.Products)
                .HasForeignKey(d => d.BrandId)
                .HasConstraintName("FK_Product_Brand");

            entity.HasOne(d => d.Cat).WithMany(p => p.Products)
                .HasForeignKey(d => d.CatId)
                .HasConstraintName("FK_Product_Category");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
