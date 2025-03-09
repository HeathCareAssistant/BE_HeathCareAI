using HealthyCareAssistant.Contract.Service.Interface;
using HealthyCareAssistant.ModelViews.ReminderModelViews;
using Microsoft.AspNetCore.Mvc;

namespace HealthyCareAssistant.Controllers
{
    [Route("api/reminders")]
    [ApiController]
    public class ReminderController : ControllerBase
    {
        private readonly IReminderService _reminderService;

        public ReminderController(IReminderService reminderService)
        {
            _reminderService = reminderService;
        }

        /// <summary>
        /// Lấy danh sách nhắc nhở của một user cụ thể
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetAllRemindersByUser(int userId)
        {
            var reminders = await _reminderService.GetAllRemindersByUserIdAsync(userId);
            return Ok(reminders);
        }

        /// <summary>
        /// Tạo nhắc nhở (One-Time hoặc Lặp Lại)
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> CreateReminder(int userId, [FromBody] CreateReminderRequest request)
        {
            var result = await _reminderService.CreateReminderAsync(userId, request);
            return Ok(new { message = result });
        }

        /// <summary>
        /// Lấy chi tiết một nhắc nhở cụ thể (bao gồm cả danh sách thuốc)
        /// </summary>
        [HttpGet("{reminderId}")]
        public async Task<IActionResult> GetReminderDetail(int reminderId)
        {
            var reminder = await _reminderService.GetReminderDetailAsync(reminderId);
            if (reminder == null)
            {
                return NotFound(new { message = "Không tìm thấy nhắc nhở" });
            }
            return Ok(reminder);
        }

        /// <summary>
        /// Bật/Tắt trạng thái hoạt động của nhắc nhở
        /// </summary>
        [HttpPut("{reminderId}/toggle-status")]
        public async Task<IActionResult> ToggleReminderStatus(int reminderId)
        {
            var success = await _reminderService.ToggleReminderStatusAsync(reminderId);
            if (!success)
            {
                return BadRequest(new { message = "Không thể cập nhật trạng thái nhắc nhở" });
            }
            return Ok(new { message = "Cập nhật trạng thái thành công" });
        }

        /// <summary>
        /// Xóa một nhắc nhở
        /// </summary>
        [HttpDelete("{reminderId}")]
        public async Task<IActionResult> DeleteReminder(int reminderId)
        {
            var success = await _reminderService.DeleteReminderAsync(reminderId);
            if (!success)
            {
                return BadRequest(new { message = "Không thể xóa nhắc nhở" });
            }
            return Ok(new { message = "Xóa nhắc nhở thành công" });
        }

        [HttpPut("{reminderId}")]
        public async Task<IActionResult> UpdateReminder(int reminderId, [FromBody] UpdateReminderRequest request)
        {
            var result = await _reminderService.UpdateReminderAsync(reminderId, request);
            return result ? Ok(new { message = "Nhắc nhở đã được cập nhật thành công." }) : NotFound("Không tìm thấy nhắc nhở.");
        }

    }

}
