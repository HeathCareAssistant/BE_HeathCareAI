using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthyCareAssistant.ModelViews.Chatbot
{
    public class ChatResponseModel
    {
        public string ChatId { get; set; }
        public string ConversationId { get; set; }
        public string Response { get; set; }
        public dynamic Status { get; set; }
    }
}
