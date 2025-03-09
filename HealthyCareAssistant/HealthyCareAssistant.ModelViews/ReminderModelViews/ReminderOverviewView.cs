using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthyCareAssistant.ModelViews.ReminderModelViews
{
    public class ReminderOverviewView
    {
        public int ReminderId { get; set; }
        public string Note { get; set; }
        public TimeOnly ReminderTime { get; set; }
        public string RepeatDays { get; set; }
        public bool IsOneTime { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int DrugCount { get; set; } // Số lượng loại thuốc
        public List<string> DrugNames { get; set; } // Danh sách tên các loại thuốc
    }

}
