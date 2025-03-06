using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthyCareAssistant.ModelViews.MedicineCabinetModelViews
{
    public class MedicineCabinetView
    {
        public int CabinetId { get; set; }
        public string CabinetName { get; set; }
        public string Description { get; set; }
        public int DrugsCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
