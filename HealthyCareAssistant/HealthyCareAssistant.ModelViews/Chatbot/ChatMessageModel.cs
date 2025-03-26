using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthyCareAssistant.ModelViews.Chatbot
{
    public class ChatMessageModel
    {
        public string MessageId { get; set; }
        public string ChatId { get; set; }
        public string ConversationId { get; set; }
        public string Content { get; set; }
        public string ContentType { get; set; }
        public string Role { get; set; }
    }
}
