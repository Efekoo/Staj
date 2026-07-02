using Backend.API.Hubs;
using Backend.Core.Interfaces;
using Backend.DataAccess.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace Backend.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly AppDbContext _context;
    private readonly IHubContext<LeaderboardHub> _leaderboardHub;
    public UserController(AppDbContext context, IUserService userService, IHubContext<LeaderboardHub> leaderboardHub)
    {
        _userService = userService;
        _context = context;
        _leaderboardHub = leaderboardHub;
    }


    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMe()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

        int userId = int.Parse(userIdStr);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return NotFound();

        return Ok(new
        {
            user.Id,
            user.Username,
            user.Email,
            user.Coin,
            user.XP,
            user.Level
        });
    }
    [HttpPost("add-xp")]
    [Authorize]
    public async Task<IActionResult> AddXP([FromBody] int xpToAdd)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdStr == null) return Unauthorized();

        int userId = int.Parse(userIdStr);

        await _userService.AddXPAsync(userId, xpToAdd);

        var standings = await _userService.GetTopUsersAsync();
        await _leaderboardHub.Clients.All.SendAsync("LeaderboardUpdated", standings);

        var updatedUser = await _userService.GetUserByIdAsync(userId);
        return Ok(new
        {
            updatedUser.Level,
            updatedUser.XP,
            Message = $"{xpToAdd} XP eklendi. Yeni seviye: {updatedUser.Level}"
        });
    }
}