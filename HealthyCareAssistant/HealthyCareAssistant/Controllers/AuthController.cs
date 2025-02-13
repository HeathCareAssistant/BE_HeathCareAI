using HealthyCareAssistant.Contract.Service.Interface;
using HealthyCareAssistant.ModelViews.AuthModelViews;
using Microsoft.AspNetCore.Mvc;

namespace HealthyCareAssistant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService) 
        {
            _userService = userService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModelViews model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.RegisterAsync(model);
            return Ok(new { message = result });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModelViews model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var token = await _userService.LoginAsync(model);
            if (token == "Invalid email or password.")
            {
                return Unauthorized(new { message = token });
            }

            return Ok(new { token });
        }
    }
}
