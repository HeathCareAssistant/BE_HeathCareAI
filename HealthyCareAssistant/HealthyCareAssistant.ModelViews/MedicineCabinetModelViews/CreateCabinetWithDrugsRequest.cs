using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthyCareAssistant.ModelViews.MedicineCabinetModelViews
{
    public class CreateCabinetWithDrugsRequest
    {
        public string CabinetName { get; set; }
        public string Description { get; set; }
        public List<CabinetDrugDto> DrugList { get; set; } = new List<CabinetDrugDto>();
    }

}
