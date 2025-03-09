using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthyCareAssistant.ModelViews.ReminderModelViews
{
    public class CreateReminderRequest
    {
        public string Note { get; set; }
        public TimeOnly ReminderTime { get; set; }
        public List<int>? RepeatDays { get; set; } // Nếu là OneTime thì null
        public List<string> DrugIds { get; set; }
    }

}
