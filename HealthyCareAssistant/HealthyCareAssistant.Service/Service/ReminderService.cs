using HealthyCareAssistant.Contact.Repo.Entity;
using HealthyCareAssistant.Contact.Repo.IUOW;
using HealthyCareAssistant.Contract.Service.Interface;
using HealthyCareAssistant.ModelViews.DrugModelViews;
using HealthyCareAssistant.ModelViews.ReminderModelViews;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthyCareAssistant.Service.Service
{
    public class ReminderService : IReminderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Reminder> _reminderRepo;
        private readonly IGenericRepository<ReminderDrug> _reminderDrugRepo;
        //private readonly IFcmService _fcmService;

        public ReminderService(IUnitOfWork unitOfWork)//, IFcmService fcmService)
        {
            _unitOfWork = unitOfWork;
            _reminderRepo = _unitOfWork.GetRepository<Reminder>();
            _reminderDrugRepo = _unitOfWork.GetRepository<ReminderDrug>();
            //_fcmService = fcmService;
        }

        public async Task<IEnumerable<ReminderOverviewView>> GetAllRemindersByUserIdAsync(int userId)
        {
            return await _reminderRepo.Entities
                .Where(r => r.UserId == userId)
                .Select(r => new ReminderOverviewView
                {
                    ReminderId = r.ReminderId,
                    ReminderTime = (TimeOnly)r.ReminderTime,
                    Note = r.Note,
                    RepeatDays = r.RepeatDays,
                    IsOneTime = (bool)r.IsOneTime,
                    IsActive = (bool)r.IsActive,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt,
                    DrugCount = r.ReminderDrugs.Count(),
                    DrugNames = r.ReminderDrugs.Select(rd => rd.Drug.TenThuoc).ToList() // Lấy danh sách tên thuốc
                })
                .ToListAsync();
        }

        public async Task<string> CreateReminderAsync(int userId, CreateReminderRequest request)
        {
            bool isOneTime = request.RepeatDays == null || !request.RepeatDays.Any(); // Nếu không có ngày lặp lại thì là OneTime

            var reminder = new Reminder
            {
                UserId = userId,
                Note = request.Note,
                ReminderTime = request.ReminderTime,
                RepeatDays = isOneTime ? null : string.Join(",", request.RepeatDays), // Nếu OneTime thì null, còn lại lưu dạng "2,3,4"
                IsOneTime = isOneTime,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _reminderRepo.InsertAsync(reminder);
            await _unitOfWork.SaveAsync();

            if (request.DrugIds.Any())
            {
                var reminderDrugs = request.DrugIds.Select(d => new ReminderDrug
                {
                    ReminderId = reminder.ReminderId,
                    DrugId = d
                }).ToList();

                await _reminderDrugRepo.InsertRangeAsync(reminderDrugs);
                await _unitOfWork.SaveAsync();
            }

            return isOneTime ? "Tạo nhắc nhở một lần thành công" : "Tạo nhắc nhở lặp lại thành công";
        }


        public async Task<ReminderDetailView> GetReminderDetailAsync(int reminderId)
        {
            var reminder = await _reminderRepo.Entities
                .Where(r => r.ReminderId == reminderId)
                .Include(r => r.ReminderDrugs)
                .ThenInclude(rd => rd.Drug)
                .Select(r => new ReminderDetailView
                {
                    ReminderId = r.ReminderId,
                    UserId = r.UserId,
                    Note = r.Note,
                    ReminderTime = r.ReminderTime,
                    RepeatDays = r.RepeatDays,
                    IsOneTime = r.IsOneTime,
                    IsActive = r.IsActive,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt,
                    Drugs = r.ReminderDrugs.Select(rd => new DrugModelView
                    {
                        DrugId = rd.Drug.DrugId,
                        TenThuoc = rd.Drug.TenThuoc,
                        DotPheDuyet = rd.Drug.DotPheDuyet,
                        SoQuyetDinh = rd.Drug.SoQuyetDinh,
                        PheDuyet = rd.Drug.PheDuyet,
                        HieuLuc = rd.Drug.HieuLuc,
                        SoDangKy = rd.Drug.SoDangKy,
                        HoatChat = rd.Drug.HoatChat,
                        PhanLoai = rd.Drug.PhanLoai,
                        NongDo = rd.Drug.NongDo,
                        TaDuoc = rd.Drug.TaDuoc,
                        BaoChe = rd.Drug.BaoChe,
                        DongGoi = rd.Drug.DongGoi,
                        TieuChuan = rd.Drug.TieuChuan,
                        TuoiTho = rd.Drug.TuoiTho,
                        CongTySx = rd.Drug.CongTySx,
                        CongTySxCode = rd.Drug.CongTySxCode,
                        NuocSx = rd.Drug.NuocSx,
                        DiaChiSx = rd.Drug.DiaChiSx,
                        CongTyDk = rd.Drug.CongTyDk,
                        NuocDk = rd.Drug.NuocDk,
                        DiaChiDk = rd.Drug.DiaChiDk,
                        GiaKeKhai = rd.Drug.GiaKeKhai,
                        HuongDanSuDung = rd.Drug.HuongDanSuDung,
                        HuongDanSuDungBn = rd.Drug.HuongDanSuDungBn,
                        NhomThuoc = rd.Drug.NhomThuoc,
                        IsHide = rd.Drug.IsHide,
                        Rate = rd.Drug.Rate,
                        RutSdk = rd.Drug.RutSdk,
                        FileName = rd.Drug.FileName,
                        State = rd.Drug.State,
                        CreatedAt = rd.Drug.CreatedAt,
                        UpdatedAt = rd.Drug.UpdatedAt,
                        Images = rd.Drug.Images,
                        SearchCount = rd.Drug.SearchCount
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return reminder;
        }


        // 4️⃣ Toggle trạng thái (bật/tắt)
        public async Task<bool> ToggleReminderStatusAsync(int reminderId)
        {
            var reminder = await _reminderRepo.GetByIdAsync(reminderId);
            if (reminder == null) return false;

            reminder.IsActive = !reminder.IsActive;
            await _reminderRepo.UpdateAsync(reminder);
            await _unitOfWork.SaveAsync();

            return true;
        }

        // 5️⃣ Xóa nhắc nhở
        public async Task<bool> DeleteReminderAsync(int reminderId)
        {
            var reminder = await _reminderRepo.GetByIdAsync(reminderId);
            if (reminder == null) return false;

            await _reminderDrugRepo.DeleteRangeAsync(reminder.ReminderDrugs);
            await _reminderRepo.DeleteAsync(reminderId);
            await _unitOfWork.SaveAsync();

            return true;
        }

        public async Task<bool> UpdateReminderAsync(int reminderId, UpdateReminderRequest request)
        {
            var reminder = await _reminderRepo.GetByIdAsync(reminderId);
            if (reminder == null) return false;

            // Cập nhật thông tin chung
            reminder.Note = request.Note;
            reminder.ReminderTime = request.ReminderTime;
            reminder.UpdatedAt = DateTime.UtcNow;

            // **Tự động cập nhật trạng thái OneTime hay Repeat**
            if (request.RepeatDays != null && request.RepeatDays.Any())
            {
                reminder.IsOneTime = false; // Nếu có danh sách ngày thì là "Lặp lại"
                reminder.RepeatDays = string.Join(",", request.RepeatDays);
            }
            else
            {
                reminder.IsOneTime = true; // Nếu không có ngày thì là "Lần tới"
                reminder.RepeatDays = null;
            }

            // **Cập nhật danh sách thuốc**
            var existingDrugs = _reminderDrugRepo.Entities
                .Where(rd => rd.ReminderId == reminderId)
                .ToList();

            // Xóa thuốc không còn trong danh sách mới
            var drugsToRemove = existingDrugs
                .Where(rd => !request.DrugIds.Contains(rd.DrugId))
                .ToList();

            if (drugsToRemove.Any())
            {
                await _reminderDrugRepo.DeleteRangeAsync(drugsToRemove);
            }

            // Thêm thuốc mới vào danh sách
            var drugsToAdd = request.DrugIds
                .Where(drugId => !existingDrugs.Any(rd => rd.DrugId == drugId))
                .Select(drugId => new ReminderDrug
                {
                    ReminderId = reminderId,
                    DrugId = drugId
                }).ToList();

            if (drugsToAdd.Any())
            {
                await _reminderDrugRepo.InsertRangeAsync(drugsToAdd);
            }

            await _unitOfWork.SaveAsync();
            return true;
        }





    }
}
