using Backend.DataAccess.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace Backend.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly AppDbContext _context;

    public InventoryController(AppDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetInventory()
    {
        var userIdStr=User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

        int userId = int.Parse(userIdStr);
        var inventory=await _context.InventoryItems
            .Where(i => i.UserId == userId)
            .ToListAsync();

        return Ok(inventory);
    }
}
