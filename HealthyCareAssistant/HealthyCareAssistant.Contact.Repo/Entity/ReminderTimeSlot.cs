﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace HealthyCareAssistant.Contact.Repo.Entity;

public partial class ReminderTimeSlot
{
    public int TimeSlotId { get; set; }

    public int? ReminderId { get; set; }

    public string Time { get; set; }

    public string DayOfWeek { get; set; }

    public DateOnly? SpecificDate { get; set; }

    public string Note { get; set; }

    public virtual Reminder Reminder { get; set; }
}