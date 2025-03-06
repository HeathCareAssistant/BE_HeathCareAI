using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthyCareAssistant.ModelViews.UserModelViews
{
    public class EmailMetadata
    {
        public string ToAddress { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public EmailMetadata(string toAddress, string subject, string body)
        {
            ToAddress = toAddress;
            Subject = subject;
            Body = body;
        }
    }

}
