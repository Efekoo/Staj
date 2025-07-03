using Backend.Business.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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


        [HttpPost("buy/{itemName}")]
        [Authorize]
        public async Task<IActionResult> BuyItem(string itemName)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            int userId = int.Parse(userIdStr);
            var success = await _marketService.PurchaseItemAsync(userId, itemName);

            if (!success)
                return BadRequest("Ürün bulunamadı, stokta yok veya paranız yetersiz.");

            return Ok("Ürün başarıyla satın alındı.");
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllItems()
        {
            var items = await _marketService.GetAllMarketItemsAsync();
            return Ok(items);
        }


        [HttpPost("sell/{itemName}")]
        [Authorize]
        public async Task<IActionResult> SellItem(string itemName)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            int userId = int.Parse(userIdStr);
            var success = await _marketService.SellItemAsync(userId, itemName);

            if (!success)
                return BadRequest("Ürün envanterde bulunamadı veya miktar yetersiz.");

            return Ok("Ürün başarıyla satıldı.");
        }
    }
}