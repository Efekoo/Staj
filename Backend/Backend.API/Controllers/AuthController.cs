using AutoMapper;
using Backend.API.Dtos;
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

    [HttpPost("register/{username}/{email}/{password}")]
    public async Task<IActionResult> Register(string username, string email, string password)
    {
        var user = new User
        {
            Username = username,
            Email = email,
            Coin = 100
        };

        bool result = await _userService.RegisterAsync(user, password);
        if (!result)
            return BadRequest(new { message = "Email already exists." });

        return Ok(new { message = "User registered successfully with 100 coins." });
    }


    [HttpPost("login/{email}/{password}")]
    public async Task<IActionResult> Login(string email, string password)
    {
        var user = await _userService.LoginAsync(email, password);
        if (user == null)
            return Unauthorized(new { message = "Invalid credentials." });

        var token = TokenHelper.GenerateToken(user, _config);

        return Ok(new { token });
    }
}