using Backend.DataAccess.Contexts;
using Microsoft.EntityFrameworkCore;
using Backend.Core.Entities;


namespace Backend.Business.Services
{
    public class MarketService
    {
        private readonly AppDbContext _context;
        
        public MarketService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> PurchaseItemAsync(int userId, string itemName )
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var item = await _context.MarketItems.FirstOrDefaultAsync(m => m.Name== itemName);

            if (user == null || item == null || item.Stock <= 0 || user.Coin < item.Price)
                return false;

            user.Coin -= item.Price;
            item.Stock--;

            var inventoryItem = await _context.InventoryItems
                .FirstOrDefaultAsync(i => i.UserId == userId && i.ItemName == item.Name);

            if (inventoryItem == null)
            {
                _context.InventoryItems.Add(new InventoryItem
                {
                    UserId = userId,
                    ItemName = item.Name,
                    Quantity = 1
                });
            }
            else
            {
                inventoryItem.Quantity++;
            }

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<List<MarketItem>> GetAllMarketItemsAsync()
        {
            return await _context.MarketItems.ToListAsync();
        }



        public async Task<bool> SellItemAsync(int userId, string itemName)
        {
            var inventoryItem = await _context.InventoryItems
                .FirstOrDefaultAsync(i => i.UserId == userId && i.ItemName == itemName);

            if (inventoryItem == null || inventoryItem.Quantity <= 0)
                return false;

            var marketItem = await _context.MarketItems
                .FirstOrDefaultAsync(m => m.Name == itemName);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (marketItem == null || user == null)
                return false;

            inventoryItem.Quantity--;
            marketItem.Stock++;
            user.Coin += marketItem.Price;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
