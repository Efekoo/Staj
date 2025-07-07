using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Core.DTOs
{
    public class MarketItemCreateDto
    {
        public string Name { get; set; } = null!;
        public int Price { get; set; }
        public int Stock { get; set; }
    }

    public class BuyItemDto
    {
        public string ItemName { get; set; } = null!;
        public int Quantity { get; set; }
    }

    public class SellItemDto
    {
        public string ItemName { get; set; } = null!;
        public int Quantity { get; set; }
    }
    public class UserLeaderboardDto
    {
        public int Rank { get; set; }
        public string Username { get; set; } = null!;
        public int Level { get; set; }
        public int XP { get; set; }
    }
    public class UserLoginDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
    public class UserRegisterDto
    {
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

}
