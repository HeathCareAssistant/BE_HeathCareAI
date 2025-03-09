using HealthyCareAssistant.ModelViews.ReminderModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthyCareAssistant.Contract.Service.Interface
{
    public interface IReminderService
    {
        Task<IEnumerable<ReminderOverviewView>> GetAllRemindersByUserIdAsync(int userId);
        Task<string> CreateReminderAsync(int userId, CreateReminderRequest request);
        Task<ReminderDetailView> GetReminderDetailAsync(int reminderId);
        Task<bool> ToggleReminderStatusAsync(int reminderId);
        Task<bool> DeleteReminderAsync(int reminderId);
        Task<bool> UpdateReminderAsync(int reminderId, UpdateReminderRequest request);

    }
}
