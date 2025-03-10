﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace HealthyCareAssistant.Contact.Repo.Entity;

public partial class Drug
{
    public string DrugId { get; set; }

    public string TenThuoc { get; set; }

    public string DotPheDuyet { get; set; }

    public string SoQuyetDinh { get; set; }

    public DateOnly? PheDuyet { get; set; }

    public string HieuLuc { get; set; }

    public string SoDangKy { get; set; }

    public string HoatChat { get; set; }

    public string PhanLoai { get; set; }

    public string NongDo { get; set; }

    public string TaDuoc { get; set; }

    public string BaoChe { get; set; }

    public string DongGoi { get; set; }

    public string TieuChuan { get; set; }

    public string TuoiTho { get; set; }

    public string CongTySx { get; set; }

    public string CongTySxCode { get; set; }

    public string NuocSx { get; set; }

    public string DiaChiSx { get; set; }

    public string CongTyDk { get; set; }

    public string NuocDk { get; set; }

    public string DiaChiDk { get; set; }

    public double? GiaKeKhai { get; set; }

    public string HuongDanSuDung { get; set; }

    public string HuongDanSuDungBn { get; set; }

    public string NhomThuoc { get; set; }

    public bool? IsHide { get; set; }

    public double? Rate { get; set; }

    public bool? RutSdk { get; set; }

    public string FileName { get; set; }

    public int? State { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string Images { get; set; }

    public int? SearchCount { get; set; }

    public virtual ICollection<MedicineCabinetDrug> MedicineCabinetDrugs { get; set; } = new List<MedicineCabinetDrug>();

    public virtual ICollection<ReminderDrug> ReminderDrugs { get; set; } = new List<ReminderDrug>();
}