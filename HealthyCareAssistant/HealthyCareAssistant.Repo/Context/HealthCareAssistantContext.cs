﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using HealthyCareAssistant.Contact.Repo.Entity;
using Microsoft.EntityFrameworkCore;

namespace HealthyCareAssistant.Repo.Context;

public partial class HealthCareAssistantContext : DbContext
{
    public HealthCareAssistantContext(DbContextOptions<HealthCareAssistantContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<ChatbotService> ChatbotServices { get; set; }

    public virtual DbSet<HealthProfile> HealthProfiles { get; set; }

    public virtual DbSet<MessageHistory> MessageHistories { get; set; }

    public virtual DbSet<Packaging> Packagings { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Reminder> Reminders { get; set; }

    public virtual DbSet<ReminderProduct> ReminderProducts { get; set; }

    public virtual DbSet<ReminderTimeSlot> ReminderTimeSlots { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A2B3A0DFFAB");

            entity.HasIndex(e => e.Name, "UQ__Categori__737584F68849DDED").IsUnique();

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
        });

        modelBuilder.Entity<ChatbotService>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__ChatbotS__C51BB0EA492287F2");

            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");
            entity.Property(e => e.ServiceType)
                .IsRequired()
                .HasMaxLength(50);
        });

        modelBuilder.Entity<HealthProfile>(entity =>
        {
            entity.HasKey(e => e.ProfileId).HasName("PK__HealthPr__290C88844BA2F2C1");

            entity.Property(e => e.ProfileId).HasColumnName("ProfileID");
            entity.Property(e => e.Bmi)
                .HasComputedColumnSql("([Weight]/((([Height]/(100.0))*[Height])/(100.0)))", false)
                .HasColumnType("numeric(38, 21)")
                .HasColumnName("BMI");
            entity.Property(e => e.Height).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Weight).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.User).WithMany(p => p.HealthProfiles)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__HealthPro__UserI__5DCAEF64");
        });

        modelBuilder.Entity<MessageHistory>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__MessageH__C87C037C3B0E29C0");

            entity.ToTable("MessageHistory");

            entity.Property(e => e.MessageId).HasColumnName("MessageID");
            entity.Property(e => e.ChatbotServiceId).HasColumnName("ChatbotServiceID");
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.MessageType)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.SentAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.ChatbotService).WithMany(p => p.MessageHistories)
                .HasForeignKey(d => d.ChatbotServiceId)
                .HasConstraintName("FK__MessageHi__Chatb__6E01572D");

            entity.HasOne(d => d.User).WithMany(p => p.MessageHistories)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__MessageHi__UserI__6D0D32F4");
        });

        modelBuilder.Entity<Packaging>(entity =>
        {
            entity.HasKey(e => e.PackagingId).HasName("PK__Packagin__BD507F585CF64473");

            entity.ToTable("Packaging");

            entity.HasIndex(e => e.Code, "UQ__Packagin__A25C5AA765583FF0").IsUnique();

            entity.Property(e => e.PackagingId).HasColumnName("PackagingID");
            entity.Property(e => e.Code)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(d => d.Product).WithMany(p => p.Packagings)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Packaging__Produ__5AEE82B9");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6EDEBD1EBF0");

            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Dosage).HasMaxLength(100);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Products__Catego__5535A963");
        });

        modelBuilder.Entity<Reminder>(entity =>
        {
            entity.HasKey(e => e.ReminderId).HasName("PK__Reminder__01A830A7EC73A239");

            entity.Property(e => e.ReminderId).HasColumnName("ReminderID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Reminders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Reminders__UserI__60A75C0F");
        });

        modelBuilder.Entity<ReminderProduct>(entity =>
        {
            entity.HasKey(e => e.ReminderProductId).HasName("PK__Reminder__D5B541A54D54749C");

            entity.Property(e => e.ReminderProductId).HasColumnName("ReminderProductID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.ReminderId).HasColumnName("ReminderID");

            entity.HasOne(d => d.Product).WithMany(p => p.ReminderProducts)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__ReminderP__Produ__656C112C");

            entity.HasOne(d => d.Reminder).WithMany(p => p.ReminderProducts)
                .HasForeignKey(d => d.ReminderId)
                .HasConstraintName("FK__ReminderP__Remin__6477ECF3");
        });

        modelBuilder.Entity<ReminderTimeSlot>(entity =>
        {
            entity.HasKey(e => e.TimeSlotId).HasName("PK__Reminder__41CC1F52847A491F");

            entity.Property(e => e.TimeSlotId).HasColumnName("TimeSlotID");
            entity.Property(e => e.DayOfWeek).HasMaxLength(50);
            entity.Property(e => e.ReminderId).HasColumnName("ReminderID");
            entity.Property(e => e.Time)
                .IsRequired()
                .HasMaxLength(10);

            entity.HasOne(d => d.Reminder).WithMany(p => p.ReminderTimeSlots)
                .HasForeignKey(d => d.ReminderId)
                .HasConstraintName("FK__ReminderT__Remin__68487DD7");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3A7738AD5C");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B616098552E67").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleName)
                .IsRequired()
                .HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC55B967F7");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105347A0B40D9").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(15);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__Users__RoleID__4D94879B");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}