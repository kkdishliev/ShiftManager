using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ShiftManager.Domain.Entities;

namespace ShiftManager.Infrastructure.Data;

public partial class ShiftManagerDBContext : DbContext
{
    public ShiftManagerDBContext(DbContextOptions<ShiftManagerDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Shift> Shifts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);

            entity.HasMany(d => d.Roles).WithMany(p => p.Employees)
                .UsingEntity<Dictionary<string, object>>(
                    "EmployeeRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__EmployeeR__RoleI__5AEE82B9"),
                    l => l.HasOne<Employee>().WithMany()
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__EmployeeR__Emplo__59FA5E80"),
                    j =>
                    {
                        j.HasKey("EmployeeId", "RoleId").HasName("PK__Employee__C27FE3F0028A8E1C");
                        j.ToTable("EmployeeRole");
                    });
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Shift>(entity =>
        {
            entity.HasOne(d => d.Employee).WithMany(p => p.Shifts).HasForeignKey(d => d.EmployeeId);

            entity.HasOne(d => d.Role).WithMany(p => p.Shifts).HasForeignKey(d => d.RoleId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
