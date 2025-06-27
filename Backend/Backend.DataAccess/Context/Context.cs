using Backend.Core.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.DataAccess.Context
{
    public class Context
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<Score> Scores { get; set; }
        public DbSet<MarketItem> MarketItems { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }
    }
}
