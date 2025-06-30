using Backend.API.Dtos;
using Backend.Core.Entities;
using Backend.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto dto)
        {
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email
            };

            bool result = await _userService.RegisterAsync(user, dto.Password);
            if (!result) return BadRequest(new { message = "Email already exists." });

            return Ok(new { message = "User registered successfully." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            var user = await _userService.LoginAsync(dto.Email, dto.Password);
            if (user == null) return Unauthorized(new { message = "Invalid credentials." });

            return Ok(new { message = "Login successful", user.Id, user.Username });
        }
    }
}
