﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace HealthyCareAssistant.Contact.Repo.Entity;

public partial class Reminder
{
    public int ReminderId { get; set; }

    public int? UserId { get; set; }

    public string Note { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<ReminderProduct> ReminderProducts { get; set; } = new List<ReminderProduct>();

    public virtual ICollection<ReminderTimeSlot> ReminderTimeSlots { get; set; } = new List<ReminderTimeSlot>();

    public virtual User User { get; set; }
}