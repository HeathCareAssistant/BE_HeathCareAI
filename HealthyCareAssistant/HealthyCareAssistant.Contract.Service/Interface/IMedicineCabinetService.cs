using HealthyCareAssistant.ModelViews.MedicineCabinetModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthyCareAssistant.Contract.Service.Interface
{
    public interface IMedicineCabinetService
    {
        Task<string> CreateCabinetAsync(int userId, string cabinetName, string description);
        Task<string> CreateCabinetWithDrugsAsync(int userId, CreateCabinetWithDrugsRequest request);
        Task<IEnumerable<MedicineCabinetView>> GetUserCabinetsAsync(int userId);
        Task<string> UpdateCabinetAsync(int cabinetId, string cabinetName, string description);
        Task<string> UpdateCabinetDrugsAsync(int cabinetId, List<string> finalDrugIds);
        Task<string> DeleteCabinetAsync(int cabinetId);
        Task<IEnumerable<CabinetDrugDetailView>> GetCabinetDrugsAsync(int cabinetId);
    }

}
