using HealthyCareAssistant.Contact.Repo.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthyCareAssistant.Service.Config
{
    public class ChatbotSession
    {
        public int ChatbotSessionId { get; set; }
        public int UserId { get; set; }
        public string ConversationId { get; set; } // ID từ Coze
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual User User { get; set; }
        public virtual ICollection<MessageHistory> Messages { get; set; } = new List<MessageHistory>();
    }
}
