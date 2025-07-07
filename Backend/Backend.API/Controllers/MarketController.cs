using Backend.Business.Dtos;
using Backend.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarketController : ControllerBase
    {
        private readonly MarketService _marketService;

        public MarketController(MarketService marketService)
        {
            _marketService = marketService;
        }


        [HttpPost("buy/{itemName}/{quantity}")]
        [Authorize]
        public async Task<IActionResult> BuyItem(string itemName, int quantity)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            int userId = int.Parse(userIdStr);
            var result = await _marketService.BuyItemAsync(userId, itemName, quantity);

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllItems()
        {
            var items = await _marketService.GetAllMarketItemsAsync();
            return Ok(items);
        }


        [HttpPost("sell/{itemName}/{quantity}")]
        [Authorize]
        public async Task<IActionResult> SellItem(string itemName, int quantity)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            int userId = int.Parse(userIdStr);
            var result = await _marketService.SellItemAsync(userId, itemName, quantity);

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }
    }
}