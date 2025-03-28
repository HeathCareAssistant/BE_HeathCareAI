﻿using HealthyCareAssistant.Contact.Repo.Entity;
using HealthyCareAssistant.Contact.Repo.IUOW;
using HealthyCareAssistant.Contract.Service.Interface;
using HealthyCareAssistant.ModelViews.DrugModelViews;
using HealthyCareAssistant.ModelViews.FirebaseSetting;
using HealthyCareAssistant.Service.Service.firebase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthyCareAssistant.Service.Service
{
    public class DrugService : IDrugService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Drug> _drugRepo;
        private readonly IFirebaseSetting _firebaseSettings;
        public DrugService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _drugRepo = _unitOfWork.GetRepository<Drug>();
        }


        public async Task<(IEnumerable<DrugModelView> drugs, int totalElement, int totalPage)> GetAllDrugsPaginatedAsync(int page, int pageSize)
        {
            var totalElement = await _drugRepo.Entities.CountAsync(); // Đếm tổng số phần tử
            var totalPage = (int)Math.Ceiling(totalElement / (double)pageSize); // Tính tổng số trang

            var drugs = await _drugRepo.Entities
                .OrderBy(d => d.DrugId) // Đảm bảo sắp xếp để phân trang đúng
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new DrugModelView
                {
                    DrugId = d.DrugId,
                    TenThuoc = d.TenThuoc,
                    DotPheDuyet = d.DotPheDuyet,
                    SoQuyetDinh = d.SoQuyetDinh,
                    PheDuyet = d.PheDuyet,
                    HieuLuc = d.HieuLuc,
                    SoDangKy = d.SoDangKy,
                    HoatChat = d.HoatChat,
                    PhanLoai = d.PhanLoai,
                    NongDo = d.NongDo,
                    TaDuoc = d.TaDuoc,
                    BaoChe = d.BaoChe,
                    DongGoi = d.DongGoi,
                    TieuChuan = d.TieuChuan,
                    TuoiTho = d.TuoiTho,
                    CongTySx = d.CongTySx,
                    CongTySxCode = d.CongTySxCode,
                    NuocSx = d.NuocSx,
                    DiaChiSx = d.DiaChiSx,
                    CongTyDk = d.CongTyDk,
                    NuocDk = d.NuocDk,
                    DiaChiDk = d.DiaChiDk,
                    GiaKeKhai = d.GiaKeKhai,
                    HuongDanSuDung = d.HuongDanSuDung,
                    HuongDanSuDungBn = d.HuongDanSuDungBn,
                    NhomThuoc = d.NhomThuoc,
                    IsHide = d.IsHide,
                    Rate = d.Rate,
                    RutSdk = d.RutSdk,
                    FileName = d.FileName,
                    State = d.State,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt,
                    Images =d.Images,
                    SearchCount = d.SearchCount
                })
                .ToListAsync();

            return (drugs, totalElement, totalPage);
        }


        public async Task<Drug> GetDrugByIdAsync(string id)
        {
            return await _drugRepo.GetByIdAsync(id);
        }

        public async Task<string> CreateDrugAsync(DrugModelView drugModel)
        {
            var drug = new Drug
            {
                DrugId = drugModel.DrugId,
                TenThuoc = drugModel.TenThuoc,
                DotPheDuyet = drugModel.DotPheDuyet,
                SoQuyetDinh = drugModel.SoQuyetDinh,
                PheDuyet = drugModel.PheDuyet,
                HieuLuc = drugModel.HieuLuc,
                SoDangKy = drugModel.SoDangKy,
                HoatChat = drugModel.HoatChat,
                PhanLoai = drugModel.PhanLoai,
                NongDo = drugModel.NongDo,
                TaDuoc = drugModel.TaDuoc,
                BaoChe = drugModel.BaoChe,
                DongGoi = drugModel.DongGoi,
                TieuChuan = drugModel.TieuChuan,
                TuoiTho = drugModel.TuoiTho,
                CongTySx = drugModel.CongTySx,
                CongTySxCode = drugModel.CongTySxCode,
                NuocSx = drugModel.NuocSx,
                DiaChiSx = drugModel.DiaChiSx,
                CongTyDk = drugModel.CongTyDk,
                NuocDk = drugModel.NuocDk,
                DiaChiDk = drugModel.DiaChiDk,
                GiaKeKhai = drugModel.GiaKeKhai,
                HuongDanSuDung = drugModel.HuongDanSuDung,
                HuongDanSuDungBn = drugModel.HuongDanSuDungBn,
                NhomThuoc = drugModel.NhomThuoc,
                IsHide = drugModel.IsHide,
                Rate = drugModel.Rate,
                RutSdk = drugModel.RutSdk,
                FileName = drugModel.FileName,
                State = drugModel.State,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Images = drugModel.Images,
                SearchCount = drugModel.SearchCount ?? 0
            };

            await _drugRepo.InsertAsync(drug);
            await _unitOfWork.SaveAsync();
            return "Drug created successfully";
        }

        public async Task<bool> DeleteDrugAsync(string id)
        {
            var drug = await _drugRepo.GetByIdAsync(id);
            if (drug == null) return false;

            await _drugRepo.DeleteAsync(id);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<IEnumerable<Drug>> SearchByNameAsync(string name)
        {
            return await _drugRepo.Entities
                .Where(d => d.TenThuoc != null && d.TenThuoc.ToLower().Contains(name.ToLower()))
                .ToListAsync();
        }

        public async Task<IEnumerable<Drug>> SearchByIngredientAsync(string ingredient)
        {
            return await _drugRepo.Entities
                .Where(d => d.HoatChat != null && d.HoatChat.ToLower().Contains(ingredient.ToLower()))
                .ToListAsync();
        }

        public async Task<IEnumerable<Drug>> FilterByCompanyAsync(string companyName)
        {
            return await _drugRepo.Entities
                .Where(d => d.CongTySx != null && d.CongTySx.ToLower().Contains(companyName.ToLower()))
                .ToListAsync();
        }


        public async Task<IEnumerable<Drug>> GetRelatedByIngredientAsync(string id)
        {

            var currentDrug = await _drugRepo.GetByIdAsync(id);
            if (currentDrug == null || string.IsNullOrEmpty(currentDrug.HoatChat))
            {
                return new List<Drug>();
            }


            return await _drugRepo.Entities
                .Where(d => d.DrugId != id && d.HoatChat != null && d.HoatChat.ToLower().Contains(currentDrug.HoatChat.ToLower()))
                .ToListAsync();
        }

        public async Task<IEnumerable<Drug>> GetRelatedByCompanyAsync(string id)
        {
            var currentDrug = await _drugRepo.GetByIdAsync(id);
            if (currentDrug == null || string.IsNullOrEmpty(currentDrug.CongTySx))
            {
                return new List<Drug>();
            }

            return await _drugRepo.Entities
                .Where(d => d.DrugId != id && d.CongTySx != null && d.CongTySx.ToLower().Contains(currentDrug.CongTySx.ToLower()))
                .ToListAsync();
        }


        public async Task<IEnumerable<Drug>> GetTopSearchedDrugsAsync()
        {
            return await _drugRepo.Entities
                .OrderByDescending(d => d.SearchCount)
                .Take(20)
                .ToListAsync();
        }

        public async Task IncrementSearchCountAsync(string id)
        {
            var drug = await _drugRepo.GetByIdAsync(id);
            if (drug != null)
            {
                drug.SearchCount++;
                await _drugRepo.UpdateAsync(drug);
                await _unitOfWork.SaveAsync();
            }
        }

        public async Task<bool> UpdateDrugAsync(string id, UpdateDrugModelView updatedDrug)
        {
            var drug = await _drugRepo.GetByIdAsync(id);
            if (drug == null) return false;

            drug.TenThuoc = updatedDrug.TenThuoc;
            drug.HoatChat = updatedDrug.HoatChat;
            drug.PhanLoai = updatedDrug.PhanLoai;
            drug.NongDo = updatedDrug.NongDo;
            drug.TaDuoc = updatedDrug.TaDuoc;
            drug.BaoChe = updatedDrug.BaoChe;
            drug.DongGoi = updatedDrug.DongGoi;
            drug.TieuChuan = updatedDrug.TieuChuan;
            drug.TuoiTho = updatedDrug.TuoiTho;
            drug.CongTySx = updatedDrug.CongTySx;
            drug.CongTySxCode = updatedDrug.CongTySxCode;
            drug.NuocSx = updatedDrug.NuocSx;
            drug.DiaChiSx = updatedDrug.DiaChiSx;
            drug.CongTyDk = updatedDrug.CongTyDk;
            drug.NuocDk = updatedDrug.NuocDk;
            drug.DiaChiDk = updatedDrug.DiaChiDk;
            drug.GiaKeKhai = updatedDrug.GiaKeKhai;
            drug.HuongDanSuDung = updatedDrug.HuongDanSuDung;
            drug.HuongDanSuDungBn = updatedDrug.HuongDanSuDungBn;
            drug.NhomThuoc = updatedDrug.NhomThuoc;
            drug.Rate = updatedDrug.Rate;
            drug.RutSdk = updatedDrug.RutSdk;
            drug.FileName = updatedDrug.FileName;
            drug.State = updatedDrug.State;
            drug.Images = updatedDrug.Images;

            // Cập nhật thời gian
            drug.UpdatedAt = DateTime.UtcNow;

            await _drugRepo.UpdateAsync(drug);
            await _unitOfWork.SaveAsync();
            return true;
        }


        public async Task<IEnumerable<DrugModelView>> GetTopNewRegisteredDrugsAsync()
        {
            return await _drugRepo.Entities
                .Where(d => d.State == 202)
                .OrderByDescending(d => d.PheDuyet ?? DateOnly.MinValue) // Ưu tiên theo ngày phê duyệt
                .ThenByDescending(d => d.CreatedAt) // Nếu ngày phê duyệt giống nhau thì sắp theo CreatedAt
                .Take(10)
                .Select(d => new DrugModelView
                {
                    DrugId = d.DrugId,
                    TenThuoc = d.TenThuoc,
                    DotPheDuyet = d.DotPheDuyet,
                    SoQuyetDinh = d.SoQuyetDinh,
                    PheDuyet = d.PheDuyet,
                    HieuLuc = d.HieuLuc,
                    SoDangKy = d.SoDangKy,
                    HoatChat = d.HoatChat,
                    PhanLoai = d.PhanLoai,
                    NongDo = d.NongDo,
                    TaDuoc = d.TaDuoc,
                    BaoChe = d.BaoChe,
                    DongGoi = d.DongGoi,
                    TieuChuan = d.TieuChuan,
                    TuoiTho = d.TuoiTho,
                    CongTySx = d.CongTySx,
                    CongTySxCode = d.CongTySxCode,
                    NuocSx = d.NuocSx,
                    DiaChiSx = d.DiaChiSx,
                    CongTyDk = d.CongTyDk,
                    NuocDk = d.NuocDk,
                    DiaChiDk = d.DiaChiDk,
                    GiaKeKhai = d.GiaKeKhai,
                    HuongDanSuDung = d.HuongDanSuDung,
                    HuongDanSuDungBn = d.HuongDanSuDungBn,
                    NhomThuoc = d.NhomThuoc,
                    IsHide = d.IsHide,
                    Rate = d.Rate,
                    RutSdk = d.RutSdk,
                    FileName = d.FileName,
                    State = d.State,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt,
                    Images = d.Images,
                    SearchCount = d.SearchCount
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<DrugModelView>> GetTopWithdrawnDrugsAsync()
        {
            return await _drugRepo.Entities
                .Where(d => d.RutSdk == true)
                .OrderByDescending(d => d.UpdatedAt)
                .Take(10)
                .Select(d => new DrugModelView
                {
                    DrugId = d.DrugId,
                    TenThuoc = d.TenThuoc,
                    DotPheDuyet = d.DotPheDuyet,
                    SoQuyetDinh = d.SoQuyetDinh,
                    PheDuyet = d.PheDuyet,
                    HieuLuc = d.HieuLuc,
                    SoDangKy = d.SoDangKy,
                    HoatChat = d.HoatChat,
                    PhanLoai = d.PhanLoai,
                    NongDo = d.NongDo,
                    TaDuoc = d.TaDuoc,
                    BaoChe = d.BaoChe,
                    DongGoi = d.DongGoi,
                    TieuChuan = d.TieuChuan,
                    TuoiTho = d.TuoiTho,
                    CongTySx = d.CongTySx,
                    CongTySxCode = d.CongTySxCode,
                    NuocSx = d.NuocSx,
                    DiaChiSx = d.DiaChiSx,
                    CongTyDk = d.CongTyDk,
                    NuocDk = d.NuocDk,
                    DiaChiDk = d.DiaChiDk,
                    GiaKeKhai = d.GiaKeKhai,
                    HuongDanSuDung = d.HuongDanSuDung,
                    HuongDanSuDungBn = d.HuongDanSuDungBn,
                    NhomThuoc = d.NhomThuoc,
                    IsHide = d.IsHide,
                    Rate = d.Rate,
                    RutSdk = d.RutSdk,
                    FileName = d.FileName,
                    State = d.State,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt,
                    Images = d.Images,
                    SearchCount = d.SearchCount
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<object>> GetTopCompaniesByDrugsAsync()
        {
            return await _drugRepo.Entities
                .GroupBy(d => d.CongTySx)
                .Select(g => new
                {
                    CompanyName = g.Key,
                    TotalDrugs = g.Count()
                })
                .OrderByDescending(g => g.TotalDrugs)
                .Take(10)
                .ToListAsync();
        }

        public async Task<(IEnumerable<DrugModelView> drugs, int totalElement, int totalPage)> FilterByDrugGroupAsync(string group, int page, int pageSize)
        {
            var query = _drugRepo.Entities
        .Where(d => d.NhomThuoc != null && d.NhomThuoc.ToLower().Contains(group.ToLower()));

            int totalElement = await query.CountAsync(); // Tổng số phần tử
            int totalPage = (int)Math.Ceiling(totalElement / (double)pageSize); // Tổng số trang

            var drugs = await query
                .OrderBy(d => d.DrugId) // Sắp xếp để phân trang đúng
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new DrugModelView
                {
                    DrugId = d.DrugId,
                    TenThuoc = d.TenThuoc,
                    DotPheDuyet = d.DotPheDuyet,
                    SoQuyetDinh = d.SoQuyetDinh,
                    PheDuyet = d.PheDuyet,
                    HieuLuc = d.HieuLuc,
                    SoDangKy = d.SoDangKy,
                    HoatChat = d.HoatChat,
                    PhanLoai = d.PhanLoai,
                    NongDo = d.NongDo,
                    TaDuoc = d.TaDuoc,
                    BaoChe = d.BaoChe,
                    DongGoi = d.DongGoi,
                    TieuChuan = d.TieuChuan,
                    TuoiTho = d.TuoiTho,
                    CongTySx = d.CongTySx,
                    CongTySxCode = d.CongTySxCode,
                    NuocSx = d.NuocSx,
                    DiaChiSx = d.DiaChiSx,
                    CongTyDk = d.CongTyDk,
                    NuocDk = d.NuocDk,
                    DiaChiDk = d.DiaChiDk,
                    GiaKeKhai = d.GiaKeKhai,
                    HuongDanSuDung = d.HuongDanSuDung,
                    HuongDanSuDungBn = d.HuongDanSuDungBn,
                    NhomThuoc = d.NhomThuoc,
                    IsHide = d.IsHide,
                    Rate = d.Rate,
                    RutSdk = d.RutSdk,
                    FileName = d.FileName,
                    State = d.State,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt,
                    Images = d.Images,
                    SearchCount = d.SearchCount
                })
                .ToListAsync();

            return (drugs, totalElement, totalPage);
        }

        public async Task<(IEnumerable<DrugModelView> drugs, int totalElement, int totalPage)> FilterByCategoryAsync(string category, int page, int pageSize)
        {
            var query = _drugRepo.Entities
        .Where(d => d.PhanLoai != null && d.PhanLoai.ToLower().Contains(category.ToLower()));

            int totalElement = await query.CountAsync(); // Tổng số phần tử
            int totalPage = (int)Math.Ceiling(totalElement / (double)pageSize); // Tổng số trang

            var drugs = await query
                .OrderBy(d => d.DrugId) // Sắp xếp để phân trang đúng
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new DrugModelView
                {
                    DrugId = d.DrugId,
                    TenThuoc = d.TenThuoc,
                    DotPheDuyet = d.DotPheDuyet,
                    SoQuyetDinh = d.SoQuyetDinh,
                    PheDuyet = d.PheDuyet,
                    HieuLuc = d.HieuLuc,
                    SoDangKy = d.SoDangKy,
                    HoatChat = d.HoatChat,
                    PhanLoai = d.PhanLoai,
                    NongDo = d.NongDo,
                    TaDuoc = d.TaDuoc,
                    BaoChe = d.BaoChe,
                    DongGoi = d.DongGoi,
                    TieuChuan = d.TieuChuan,
                    TuoiTho = d.TuoiTho,
                    CongTySx = d.CongTySx,
                    CongTySxCode = d.CongTySxCode,
                    NuocSx = d.NuocSx,
                    DiaChiSx = d.DiaChiSx,
                    CongTyDk = d.CongTyDk,
                    NuocDk = d.NuocDk,
                    DiaChiDk = d.DiaChiDk,
                    GiaKeKhai = d.GiaKeKhai,
                    HuongDanSuDung = d.HuongDanSuDung,
                    HuongDanSuDungBn = d.HuongDanSuDungBn,
                    NhomThuoc = d.NhomThuoc,
                    IsHide = d.IsHide,
                    Rate = d.Rate,
                    RutSdk = d.RutSdk,
                    FileName = d.FileName,
                    State = d.State,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt,
                    Images = d.Images,
                    SearchCount = d.SearchCount
                })
                .ToListAsync();

            return (drugs, totalElement, totalPage);
        }
    }
}
