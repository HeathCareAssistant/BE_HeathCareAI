using HealthyCareAssistant.Contact.Repo.Entity;
using HealthyCareAssistant.Contact.Repo.IUOW;
using HealthyCareAssistant.Contract.Service.Interface;
using HealthyCareAssistant.ModelViews.MedicineCabinetModelViews;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthyCareAssistant.Service.Service
{
    public class MedicineCabinetService : IMedicineCabinetService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<MedicineCabinet> _cabinetRepo;
        private readonly IGenericRepository<MedicineCabinetDrug> _cabinetDrugRepo;

        public MedicineCabinetService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _cabinetRepo = _unitOfWork.GetRepository<MedicineCabinet>();
            _cabinetDrugRepo = _unitOfWork.GetRepository<MedicineCabinetDrug>();
        }

        // 1️⃣ Tạo tủ thuốc (Không có thuốc)
        public async Task<string> CreateCabinetAsync(int userId, string cabinetName, string description)
        {
            var cabinet = new MedicineCabinet
            {
                UserId = userId,
                CabinetName = cabinetName,
                Description = description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _cabinetRepo.InsertAsync(cabinet);
            await _unitOfWork.SaveAsync();

            return "Tạo tủ thuốc thành công";
        }

        // 2️⃣ Tạo tủ thuốc có thuốc
        public async Task<string> CreateCabinetWithDrugsAsync(CreateCabinetWithDrugsRequest request)
        {
            var cabinet = new MedicineCabinet
            {
                UserId = request.UserId,
                CabinetName = request.CabinetName,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _cabinetRepo.InsertAsync(cabinet);
            await _unitOfWork.SaveAsync();

            if (request.DrugList.Any())
            {
                var cabinetDrugs = request.DrugList.Select(d => new MedicineCabinetDrug
                {
                    CabinetId = cabinet.CabinetId,
                    DrugId = d.DrugId,
                    Note = d.Note
                }).ToList();

                await _cabinetDrugRepo.InsertRangeAsync(cabinetDrugs);
                await _unitOfWork.SaveAsync();
            }

            return "Tạo tủ thuốc kèm thuốc thành công";
        }

        // 3️⃣ Lấy danh sách tủ thuốc của user (có số lượng thuốc)
        public async Task<IEnumerable<MedicineCabinetView>> GetUserCabinetsAsync(int userId)
        {
            return await _cabinetRepo.Entities
                .Where(c => c.UserId == userId)
                .Select(c => new MedicineCabinetView
                {
                    CabinetId = c.CabinetId,
                    CabinetName = c.CabinetName,
                    Description = c.Description,
                    CreatedAt = (DateTime)c.CreatedAt,
                    UpdatedAt = (DateTime)c.UpdatedAt,
                    DrugsCount = c.MedicineCabinetDrugs.Count()
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<CabinetDrugDetailView>> GetCabinetDrugsAsync(int cabinetId)
        {
            return await _cabinetDrugRepo.Entities
                .Where(d => d.CabinetId == cabinetId)
                .Include(d => d.Drug) // Include để lấy thông tin thuốc
                .Select(d => new CabinetDrugDetailView
                {
                    DrugId = d.Drug.DrugId,
                    TenThuoc = d.Drug.TenThuoc,
                    HoatChat = d.Drug.HoatChat,
                    PhanLoai = d.Drug.PhanLoai,
                    CongTySx = d.Drug.CongTySx,
                    GiaKeKhai = d.Drug.GiaKeKhai,
                    Note = d.Note // Ghi chú của thuốc trong tủ
                })
                .ToListAsync();
        }

        // 4️⃣ Cập nhật tủ thuốc (không cập nhật thuốc)
        public async Task<string> UpdateCabinetAsync(int cabinetId, string cabinetName, string description)
        {
            var cabinet = await _cabinetRepo.GetByIdAsync(cabinetId);
            if (cabinet == null) return "Tủ thuốc không tồn tại";

            cabinet.CabinetName = cabinetName;
            cabinet.Description = description;
            cabinet.UpdatedAt = DateTime.UtcNow;

            await _cabinetRepo.UpdateAsync(cabinet);
            await _unitOfWork.SaveAsync();

            return "Cập nhật tủ thuốc thành công";
        }

        public async Task<string> UpdateCabinetDrugsAsync(int cabinetId, List<string> finalDrugIds)
        {
            // Kiểm tra danh sách truyền vào có hợp lệ không
            if (finalDrugIds == null)
            {
                return "Danh sách thuốc không hợp lệ!";
            }

            // Lấy danh sách thuốc hiện tại trong tủ thuốc
            var existingDrugs = await _cabinetDrugRepo.Entities
                .Where(d => d.CabinetId == cabinetId)
                .ToListAsync();

            // **Tìm danh sách thuốc cần xóa** (có trong DB nhưng không có trong danh sách mới)
            var drugsToRemove = existingDrugs
                .Where(d => !finalDrugIds.Contains(d.DrugId))
                .ToList();

            if (drugsToRemove.Any())
            {
                await _cabinetDrugRepo.DeleteRangeAsync(drugsToRemove);
                await _unitOfWork.SaveAsync();
            }

            // **Tìm danh sách thuốc cần thêm** (có trong danh sách mới nhưng không có trong DB)
            var drugsToAdd = finalDrugIds
                .Where(drugId => !existingDrugs.Any(d => d.DrugId == drugId))
                .Select(drugId => new MedicineCabinetDrug
                {
                    CabinetId = cabinetId,
                    DrugId = drugId,
                    Note = "" // Có thể cập nhật từ UI nếu cần
                }).ToList();

            if (drugsToAdd.Any())
            {
                await _cabinetDrugRepo.InsertRangeAsync(drugsToAdd);
                await _unitOfWork.SaveAsync();
            }

            return "Cập nhật danh sách thuốc trong tủ thành công";
        }


        // 6️⃣ Xóa tủ thuốc (bao gồm thuốc trong bảng trung gian)
        public async Task<string> DeleteCabinetAsync(int cabinetId)
        {
            var cabinetDrugs = _cabinetDrugRepo.Entities.Where(d => d.CabinetId == cabinetId).ToList();
            foreach (var drug in cabinetDrugs)
            {
                await _cabinetDrugRepo.DeleteAsync(drug.CabinetDrugId);
            }

            await _cabinetRepo.DeleteAsync(cabinetId);
            await _unitOfWork.SaveAsync();

            return "Xóa tủ thuốc thành công";
        }
    }

}
