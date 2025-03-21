﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace HealthyCareAssistant.Contact.Repo.Entity;

public partial class User
{
    public int UserId { get; set; }

    public int? RoleId { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }

    public string PasswordHash { get; set; }

    public string PhoneNumber { get; set; }

    public string Fcmtoken { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string RefreshToken { get; set; }

    public string Otp { get; set; }

    public string Image { get; set; }

    public virtual ICollection<MedicineCabinet> MedicineCabinets { get; set; } = new List<MedicineCabinet>();

    public virtual ICollection<MessageHistory> MessageHistories { get; set; } = new List<MessageHistory>();

    public virtual ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();

    public virtual Role Role { get; set; }
}