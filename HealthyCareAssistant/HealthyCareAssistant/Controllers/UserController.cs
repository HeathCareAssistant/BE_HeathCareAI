using HealthyCareAssistant.Contact.Repo.Entity;
using HealthyCareAssistant.Contract.Service.Interface;
using HealthyCareAssistant.ModelViews.DrugModelViews;
using HealthyCareAssistant.ModelViews.UserModelViews;
using HealthyCareAssistant.Service.Service;
using HealthyCareAssistant.Service.Service.firebase;
using Microsoft.AspNetCore.Mvc;

namespace HealthyCareAssistant.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IFirebaseStorageService _firebaseStorageService;
        public UserController(IUserService userService, IFirebaseStorageService firebaseStorageService)
        {
            _userService = userService;
            _firebaseStorageService = firebaseStorageService;
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
        [HttpPost("{userid}/image/upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadUserImageAsync(string userid, [FromForm] UserUpLoadImageModel model)
        {
            if (model.File == null || model.File.Length == 0)
                return BadRequest(new { message = "Invalid IMG file" });

            var result = await _firebaseStorageService.UploadUserImageAsync(userid, model.File);
            return Ok(new { imgUrl = result });
        }

        [HttpGet("{userId}/image")]
        public async Task<IActionResult> GetUserImageUrl(string userId)
        {
            var result = await _firebaseStorageService.GetUserImageUrlAsync(userId);
            if (result.StartsWith("UserID"))
                return NotFound(new { message = result });

            return Ok(new { imageUrl = result });
        }


        [HttpDelete("{userId}/image")]
        public async Task<IActionResult> DeleteUserImage(string userId)
        {
            var result = await _firebaseStorageService.DeleteUserImageAsync(userId);
            if (result.StartsWith("UserID"))
                return NotFound(new { message = result });

            return Ok(new { message = result });
        }

    }


}
