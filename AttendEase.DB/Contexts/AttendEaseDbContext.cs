using System;
using System.Collections.Generic;
using AttendEase.DB.Models;
using Microsoft.EntityFrameworkCore;

namespace AttendEase.DB.Contexts;

public partial class AttendEaseDbContext : DbContext
{
    public AttendEaseDbContext()
    {
    }

    public AttendEaseDbContext(DbContextOptions<AttendEaseDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("user");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Role).HasColumnName("role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
