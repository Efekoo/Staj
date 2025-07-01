using Backend.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.DataAccess.Contexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Currency> Currencies => Set<Currency>();
        public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
        public DbSet<MarketItem> MarketItems => Set<MarketItem>();
    }
}
