using HealthyCareAssistant.ModelViews.DrugModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthyCareAssistant.ModelViews.ReminderModelViews
{
    public class ReminderDetailView
    {
        public int ReminderId { get; set; }
        public int? UserId { get; set; }
        public string Note { get; set; }
        public TimeOnly? ReminderTime { get; set; }
        public string RepeatDays { get; set; }
        public bool? IsOneTime { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<DrugModelView> Drugs { get; set; }
    }
}
