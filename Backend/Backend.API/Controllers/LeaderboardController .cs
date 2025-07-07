using Backend.Business.Dtos;
using Backend.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Backend.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeaderboardController : ControllerBase
{
    private readonly IUserService _userService;

    public LeaderboardController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<List<UserLeaderboardDto>>> GetTopUsers()
    {
        var topUsers = await _userService.GetTopUsersAsync();
        return Ok(topUsers);
    }
}