using HealthyCareAssistant.ModelViews.MailModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HealthyCareAssistant.Contract.Service.Interface
{
    public interface IMailService
    {
        Task<bool> SendEmailAsync(EmailData mailData);
    }
}
