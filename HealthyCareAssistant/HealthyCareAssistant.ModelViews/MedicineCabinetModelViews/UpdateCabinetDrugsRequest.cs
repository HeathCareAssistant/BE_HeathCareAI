using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthyCareAssistant.ModelViews.MedicineCabinetModelViews
{
    public class UpdateCabinetDrugsRequest
    {
        public List<string> RemoveDrugIds { get; set; } = new List<string>();
        public List<CabinetDrugDto> AddDrugs { get; set; } = new List<CabinetDrugDto>();
    }
}
