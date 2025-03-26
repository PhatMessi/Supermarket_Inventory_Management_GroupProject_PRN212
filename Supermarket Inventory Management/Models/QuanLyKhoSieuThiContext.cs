using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Supermarket_Inventory_Management.Models;

public partial class QuanLyKhoSieuThiContext : DbContext
{
    public QuanLyKhoSieuThiContext()
    {
    }

    public QuanLyKhoSieuThiContext(DbContextOptions<QuanLyKhoSieuThiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Input> Inputs { get; set; }

    public virtual DbSet<InputInfo> InputInfos { get; set; }

    public virtual DbSet<Object> Objects { get; set; }

    public virtual DbSet<Output> Outputs { get; set; }

    public virtual DbSet<OutputInfo> OutputInfos { get; set; }

    public virtual DbSet<Suplier> Supliers { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(config.GetConnectionString("DB"));
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Customer__3214EC07DD8653D6");

            entity.ToTable("Customer");

            entity.Property(e => e.ContractDate).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(20);
        });

        modelBuilder.Entity<Input>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Input__3214EC076FB6AA94");

            entity.ToTable("Input");

            entity.Property(e => e.Id).HasMaxLength(128);
            entity.Property(e => e.DateInput).HasColumnType("datetime");
        });

        modelBuilder.Entity<InputInfo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__InputInf__3214EC0762615581");

            entity.ToTable("InputInfo");

            entity.Property(e => e.Id).HasMaxLength(128);
            entity.Property(e => e.IdInput).HasMaxLength(128);
            entity.Property(e => e.IdObject).HasMaxLength(128);
            entity.Property(e => e.InputPrice).HasDefaultValue(0.0);
            entity.Property(e => e.OutputPrice).HasDefaultValue(0.0);

            entity.HasOne(d => d.IdInputNavigation).WithMany(p => p.InputInfos)
                .HasForeignKey(d => d.IdInput)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__InputInfo__IdInp__4AB81AF0");

            entity.HasOne(d => d.IdObjectNavigation).WithMany(p => p.InputInfos)
                .HasForeignKey(d => d.IdObject)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__InputInfo__IdObj__5CD6CB2B");
        });

        modelBuilder.Entity<Object>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Object__3214EC0763919737");

            entity.ToTable("Object");

            entity.Property(e => e.Id).HasMaxLength(128);
            entity.Property(e => e.Qrcode).HasColumnName("QRCode");

            entity.HasOne(d => d.IdSuplierNavigation).WithMany(p => p.Objects)
                .HasForeignKey(d => d.IdSuplier)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Object__IdSuplie__3E52440B");

            entity.HasOne(d => d.IdUnitNavigation).WithMany(p => p.Objects)
                .HasForeignKey(d => d.IdUnit)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Object__BarCode__3D5E1FD2");
        });

        modelBuilder.Entity<Output>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Output__3214EC0726714E77");

            entity.ToTable("Output");

            entity.Property(e => e.Id).HasMaxLength(128);
            entity.Property(e => e.DateOutput).HasColumnType("datetime");
        });

        modelBuilder.Entity<OutputInfo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OutputIn__3214EC07A55F1292");

            entity.ToTable("OutputInfo");

            entity.Property(e => e.Id).HasMaxLength(128);
            entity.Property(e => e.IdInputInfo).HasMaxLength(128);
            entity.Property(e => e.IdObject).HasMaxLength(128);

            entity.HasOne(d => d.IdCustomerNavigation).WithMany(p => p.OutputInfos)
                .HasForeignKey(d => d.IdCustomer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OutputInf__IdCus__5165187F");

            entity.HasOne(d => d.IdInputInfoNavigation).WithMany(p => p.OutputInfos)
                .HasForeignKey(d => d.IdInputInfo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OutputInf__IdInp__5070F446");

            entity.HasOne(d => d.IdObjectNavigation).WithMany(p => p.OutputInfos)
                .HasForeignKey(d => d.IdObject)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OutputInf__IdObj__619B8048");
        });

        modelBuilder.Entity<Suplier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Suplier__3214EC07DCD659B1");

            entity.ToTable("Suplier");

            entity.Property(e => e.ContractDate).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(20);
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Unit__3214EC0774C888AE");

            entity.ToTable("Unit");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07B90C2BDE");

            entity.Property(e => e.UserName).HasMaxLength(100);

            entity.HasOne(d => d.IdRoleNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.IdRole)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__IdRole__4316F928");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserRole__3214EC076770922E");

            entity.ToTable("UserRole");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
