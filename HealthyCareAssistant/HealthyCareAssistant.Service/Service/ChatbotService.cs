using HealthyCareAssistant.Contract.Service.Interface;
using HealthyCareAssistant.ModelViews.Chatbot;
using HealthyCareAssistant.Service.Config;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HealthyCareAssistant.Service.Service
{
    public class ChatbotService : IChatbotService
    {
        private readonly HttpClient _httpClient;
        private readonly CozeSettings _cozeSettings;

        public ChatbotService(HttpClient httpClient, IOptions<CozeSettings> cozeSettings)
        {
            _httpClient = httpClient;
            _cozeSettings = cozeSettings.Value;
        }

        /// <summary>
        /// Gửi tin nhắn đến chatbot Coze (Giống Postman)
        /// </summary>
        public async Task<ChatResponseModel> SendMessageAsync(int userId, string message)
        {
            var requestBody = new
            {
                conversation_id = "demo", // ID mặc định, có thể thay bằng userId
                bot_id = _cozeSettings.BotId,
                user_id = userId.ToString(),
                auto_save_history = true,
                additional_messages = new[]
                {
                    new { role = "user", content = message, content_type = "text" }
                },
                stream = false
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _cozeSettings.ApiKey);

            var response = await _httpClient.PostAsync($"{_cozeSettings.ApiUrl}", content);
            var responseString = await response.Content.ReadAsStringAsync();

            dynamic result = JsonConvert.DeserializeObject<dynamic>(responseString);

            return new ChatResponseModel
            {
                ChatId = result.data?.id ?? "N/A",
                ConversationId = result.data?.conversation_id ?? "N/A",
                Response = result.data?.status == "in_progress" ? "Đang xử lý, vui lòng thử lại sau!" : result.data?.content
            };
        }

        /// <summary>
        /// Lấy tin nhắn từ chatbot Coze theo Conversation ID
        /// </summary>
        public async Task<List<ChatMessageModel>> GetChatMessagesAsync(string conversationId, string chatId)
        {
            var requestUrl = $"{_cozeSettings.ApiUrl}/message/list?conversation_id={conversationId}&chat_id={chatId}";

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _cozeSettings.ApiKey);

            var response = await _httpClient.GetAsync(requestUrl);
            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(responseString);

            if (result.code != 0)
            {
                throw new Exception($"Lỗi API: {result.msg}");
            }

            var messages = new List<ChatMessageModel>();
            foreach (var msg in result.data)
            {
                messages.Add(new ChatMessageModel
                {
                    ChatId = msg.chat_id,
                    ConversationId = msg.conversation_id,
                    Content = msg.content,
                    Role = msg.role,
      
                });
            }

            return messages;
        }
    }

}

