using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthyCareAssistant.ModelViews.DrugModelViews
{
    public class DrugImageUploadModel
    {
        public IFormFile File { get; set; }
    }
}
