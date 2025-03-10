﻿using System;
using System.Collections.Generic;
using AttendEase.DB.Models;
using Microsoft.EntityFrameworkCore;

namespace AttendEase.DB.Contexts;

internal partial class AttendEaseDbContext : DbContext
{
    public AttendEaseDbContext()
    {
    }

    public AttendEaseDbContext(DbContextOptions<AttendEaseDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Attendance> Attendances { get; set; }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<SpecialDay> SpecialDays { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("attendance_pkey");

            entity.ToTable("attendance");

            entity.HasIndex(e => e.ScheduleId, "IX_attendance_schedule_id");

            entity.HasIndex(e => e.UserId, "IX_attendance_user_id");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Attended).HasColumnName("attended");
            entity.Property(e => e.ScheduleId).HasColumnName("schedule_id");
            entity.Property(e => e.Timestamp).HasColumnName("timestamp");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Schedule).WithMany(p => p.Attendances)
                .HasForeignKey(d => d.ScheduleId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("attendance_schedule_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Attendances)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("attendance_user_id_fkey");
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("contact_pkey");

            entity.ToTable("contact");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.MessageSystem).HasColumnName("message_system");
            entity.Property(e => e.MessageUser).HasColumnName("message_user");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Subject).HasColumnName("subject");
            entity.Property(e => e.Timestamp).HasColumnName("timestamp");
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("schedule_pkey");

            entity.ToTable("schedule");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AbsentAfter).HasColumnName("absent_after");
            entity.Property(e => e.AttendanceStartBefore).HasColumnName("attendance_start_before");
            entity.Property(e => e.DaysOfWeek).HasColumnName("days_of_week");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.EndTime).HasColumnName("end_time");
            entity.Property(e => e.Latitude)
                .HasPrecision(10, 8)
                .HasColumnName("latitude");
            entity.Property(e => e.LocationName).HasColumnName("location_name");
            entity.Property(e => e.LocationTolerance).HasColumnName("location_tolerance");
            entity.Property(e => e.Longitude)
                .HasPrecision(11, 8)
                .HasColumnName("longitude");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Repeat).HasColumnName("repeat");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.StartTime).HasColumnName("start_time");
        });

        modelBuilder.Entity<SpecialDay>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("special_days_pkey");

            entity.ToTable("special_days");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Recursive)
                .HasDefaultValue(false)
                .HasColumnName("recursive");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("user");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.Role).HasColumnName("role");

            entity.HasMany(d => d.Schedules).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "Assignment",
                    r => r.HasOne<Schedule>().WithMany()
                        .HasForeignKey("ScheduleId")
                        .HasConstraintName("assignment_schedule_id_fkey"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("assignment_user_id_fkey"),
                    j =>
                    {
                        j.HasKey("UserId", "ScheduleId").HasName("assignment_pkey");
                        j.ToTable("assignment");
                        j.HasIndex(new[] { "ScheduleId" }, "IX_assignment_schedule_id");
                        j.IndexerProperty<Guid>("UserId").HasColumnName("user_id");
                        j.IndexerProperty<Guid>("ScheduleId").HasColumnName("schedule_id");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
