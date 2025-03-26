using HealthyCareAssistant.Contact.Repo.Entity;
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

        public async Task<(IEnumerable<DrugModelView> drugs, int totalElement, int totalPage)> SearchDrugsAsync(string type, string value, int page, int pageSize)
        {
            var query = _drugRepo.Entities.AsQueryable();

            if (!string.IsNullOrEmpty(value))
            {
                value = value.ToLower();

                if (type == "name")
                    query = query.Where(d => d.TenThuoc != null && d.TenThuoc.ToLower().Contains(value));
                else if (type == "ingredient")
                    query = query.Where(d => d.HoatChat != null && d.HoatChat.ToLower().Contains(value));
                else if (type == "company")
                    query = query.Where(d => d.CongTySx != null && d.CongTySx.ToLower().Contains(value));
                else if (type == "category")
                    query = query.Where(d => d.PhanLoai != null && d.PhanLoai.ToLower().Contains(value));
                else if (type == "group")
                    query = query.Where(d => d.NhomThuoc != null && d.NhomThuoc.ToLower().Contains(value));
            }

            int totalElement = await query.CountAsync();
            int totalPage = (int)Math.Ceiling(totalElement / (double)pageSize);

            var drugs = await query.OrderBy(d => d.DrugId)
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

        public async Task<IEnumerable<DrugModelView>> GetTopDrugsByTypeAsync(string type)
        {
            var query = _drugRepo.Entities.AsQueryable();

            if (type == "new")
                query = query.Where(d => d.State == 202).OrderByDescending(d => d.PheDuyet ?? DateOnly.MinValue).ThenByDescending(d => d.CreatedAt);
            else if (type == "withdrawn")
                query = query.Where(d => d.RutSdk == true).OrderByDescending(d => d.UpdatedAt);
            else if (type == "top-searched")
                query = query.OrderByDescending(d => d.SearchCount);

            return await query.Take(20).Select(d => new DrugModelView
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
            }).ToListAsync();
        }



        public async Task<IEnumerable<Drug>> GetRelatedDrugsAsync(string id, string type)
        {
            var drug = await _drugRepo.GetByIdAsync(id);
            if (drug == null) return new List<Drug>();

            string compareValue = type == "ingredient" ? drug.HoatChat : drug.CongTySx;

            return await _drugRepo.Entities
                .Where(d => d.DrugId != id && (type == "ingredient"
                    ? d.HoatChat.ToLower().Contains(compareValue.ToLower())
                    : d.CongTySx.ToLower().Contains(compareValue.ToLower())))
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
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< Updated upstream
=======

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
        public async Task<IEnumerable<string>> GetAllCompaniesAsync()
        {
            return await _drugRepo.Entities
                .Where(d => !string.IsNullOrEmpty(d.CongTySx))
                .Select(d => d.CongTySx)
                .Distinct()
                .OrderBy(name => name)
                .ToListAsync();
        }

>>>>>>> Stashed changes
=======

>>>>>>> 23c07a1f76d014faf8df54e413d12f4cac51d327
=======

>>>>>>> 23c07a1f76d014faf8df54e413d12f4cac51d327
    }

}
