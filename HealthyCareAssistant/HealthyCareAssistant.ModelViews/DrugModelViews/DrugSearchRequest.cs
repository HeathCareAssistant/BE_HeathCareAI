using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthyCareAssistant.ModelViews.DrugModelViews
{
    public class DrugSearchRequest
    {
        public string? Name { get; set; }
        public string? Ingredient { get; set; }
        public string? Company { get; set; }
    }
}
