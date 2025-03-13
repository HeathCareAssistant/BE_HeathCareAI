using HealthyCareAssistant.Contract.Service.Interface;
using HealthyCareAssistant.ModelViews.Chatbot;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthyCareAssistant.Controllers
{
    [Route("api/chatbot")]
    [ApiController]
    public class ChatbotController : ControllerBase
    {
        private readonly IChatbotService _chatbotService;

        public ChatbotController(IChatbotService chatbotService)
        {
            _chatbotService = chatbotService;
        }

        /// <summary>
        /// API gửi tin nhắn đến chatbot
        /// </summary>
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] ChatRequestModel request)
        {
            if (request == null || string.IsNullOrEmpty(request.Message))
                return BadRequest("Message cannot be empty");

            var response = await _chatbotService.SendMessageAsync(request.UserId, request.Message);
            return Ok(response);
        }

        /// <summary>
        /// API lấy danh sách tin nhắn của một conversation_id
        /// </summary>
        [HttpGet("messages")]
        public async Task<IActionResult> GetMessages([FromQuery] string conversationId, [FromQuery] string chatId = null)
        {
            if (string.IsNullOrEmpty(conversationId))
                return BadRequest("conversationId is required.");

            var messages = await _chatbotService.GetChatMessagesAsync(conversationId, chatId);

            if (messages == null || !messages.Any())
                return NotFound("No messages found.");

            return Ok(messages);
        }

    }

    public class ChatRequestModel
    {
        public int UserId { get; set; }
        public string Message { get; set; }
    }
}
