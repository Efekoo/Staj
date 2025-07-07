using Backend.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Backend.DataAccess.Repositories.UserRepository;
using Backend.DataAccess.Repositories;

namespace Backend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FriendController : ControllerBase
    {
        private readonly FriendRepository _friendRepo;

        public FriendController(FriendRepository friendRepo)
        {
            _friendRepo = friendRepo;
        }

        [HttpPost("send")]
        [Authorize]
        public async Task<IActionResult> SendRequest([FromBody] int receiverId)
        {
            var senderId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var request = new FriendRequest
            {
                SenderId = senderId,
                ReceiverId = receiverId
            };

            await _friendRepo.SendFriendRequestAsync(request);
            return Ok(new { message = "Arkadaşlık isteği gönderildi." });
        }

        [HttpPost("accept")]
        [Authorize]
        public async Task<IActionResult> AcceptBySender([FromBody] int senderId)
        {
            var receiverId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _friendRepo.AcceptFriendRequestAsync(senderId, receiverId);
            return Ok(new { message = "İstek kabul edildi." });
        }

        [HttpGet("pending")]
        [Authorize]
        public async Task<IActionResult> GetPending()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var list = await _friendRepo.GetPendingRequestsAsync(userId);
            return Ok(list.Select(r => new
            {
                r.Id,
                SenderId = r.Sender.Id,
                SenderUsername = r.Sender.Username,
                r.CreatedAt
            }));
        }

        [HttpGet("list")]
        [Authorize]
        public async Task<IActionResult> GetFriends()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var friends = await _friendRepo.GetFriendsAsync(userId);
            return Ok(friends.Select(f => new
            {
                f.Id,
                f.Username,
                f.Email
            }));
        }
    }
}
