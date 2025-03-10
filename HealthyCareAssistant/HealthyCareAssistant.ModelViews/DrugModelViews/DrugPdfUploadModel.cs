﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthyCareAssistant.ModelViews.DrugModelViews
{
    public class DrugPdfUploadModel
    {
        [FromForm(Name = "file")]
        public IFormFile File { get; set; }
    }
}
