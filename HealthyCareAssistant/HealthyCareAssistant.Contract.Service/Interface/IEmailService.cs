using HealthyCareAssistant.ModelViews.UserModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthyCareAssistant.Contract.Service.Interface
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(EmailMetadata emailMetadata);
    }

}

