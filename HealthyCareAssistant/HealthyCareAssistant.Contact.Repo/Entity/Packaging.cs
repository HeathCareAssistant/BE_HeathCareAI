﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace HealthyCareAssistant.Contact.Repo.Entity;

public partial class Packaging
{
    public int PackagingId { get; set; }

    public int? ProductId { get; set; }

    public string Type { get; set; }

    public string Code { get; set; }

    public string Image { get; set; }

    public virtual Product Product { get; set; }
}