using HealthyCareAssistant.ModelViews.Chatbot;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthyCareAssistant.Contract.Service.Interface
{
    public interface IChatbotService
    {
        Task<ChatResponseModel> SendMessageAsync(int userId, string message);
        Task<List<ChatMessageModel>> GetChatMessagesAsync(string conversationId, string chatId);
    }
}
