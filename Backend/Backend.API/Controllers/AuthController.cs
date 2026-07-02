using AutoMapper;
using Backend.Core.DTOs;
using Backend.Core.Entities;
using Backend.Core.Helpers;
using Backend.Core.Interfaces;
using Backend.DataAccess.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IConfiguration _config;
    private readonly IMapper _mapper;

    public AuthController(IUserService userService, IConfiguration config, IMapper mapper)
    {
        _userService = userService;
        _config = config;
        _mapper = mapper;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
    {
        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            Coin = 100
        };

        bool result = await _userService.RegisterAsync(user, dto.Password);
        if (!result)
            return BadRequest(new { message = "Email already exists." });

        return Ok(new { message = "User registered successfully with 100 coins." });
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
    {
        var user = await _userService.LoginAsync(dto.Email, dto.Password);
        if (user == null)
            return Unauthorized(new { message = "Invalid credentials." });

        var token = TokenHelper.GenerateToken(user, _config);

        return Ok(new { token });
    }
}