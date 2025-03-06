using HealthyCareAssistant.Contact.Repo.Entity;
using HealthyCareAssistant.Contract.Service.Interface;
using HealthyCareAssistant.ModelViews.UserModelViews;
using HealthyCareAssistant.Service.Service;
using Microsoft.AspNetCore.Mvc;

namespace HealthyCareAssistant.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // 1️ Lấy danh sách User (RoleId = 2)
        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var (users, totalElement, totalPage) = await _userService.GetAllUsersPaginatedAsync(page, pageSize);
            return Ok(new
            {
                totalElement,
                totalPage,
                currentPage = page,
                pageSize,
                data = users
            });
        }

        // 2️ Lấy User theo ID
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null) return NotFound();
            return Ok(user);
        }

        // 3️ Tạo User mới
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateRequest request)
        {
            return Ok(await _userService.CreateUserAsync(request));
        }

        // 4️ Cập nhật User
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UserUpdateRequest request)
        {
            return Ok(await _userService.UpdateUserAsync(userId, request));
        }

        // 5️ Xóa User
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            return Ok(await _userService.DeleteUserAsync(userId));
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string keyword)
        {
            return Ok(await _userService.SearchUsersAsync(keyword));
        }
    }


}
