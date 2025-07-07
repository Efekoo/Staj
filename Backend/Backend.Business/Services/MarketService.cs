using Backend.DataAccess.Contexts;
using Microsoft.EntityFrameworkCore;
using Backend.Core.Entities;
using Backend.Business.Results;

namespace Backend.Business.Services
{
    public class MarketService
    {
        private readonly AppDbContext _context;
        
        public MarketService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OperationResult> BuyItemAsync(int userId, string itemName, int quantity)
        {
            var user = await _context.Users.FindAsync(userId);
            var item = await _context.MarketItems.FirstOrDefaultAsync(i => i.Name == itemName);

            if (user == null || item == null)
                return OperationResult.Failure("Kullanıcı veya ürün bulunamadı.");

            if (item.Stock < quantity)
                return OperationResult.Failure("Yeterli stok yok.");

            var totalCost = item.Price * quantity;
            if (user.Coin < totalCost)
                return OperationResult.Failure("Yetersiz bakiye.");

            item.Stock -= quantity;
            user.Coin -= totalCost;

            var inventoryItem = await _context.InventoryItems
                .FirstOrDefaultAsync(i => i.UserId == userId && i.ItemName == itemName);

            if (inventoryItem != null)
                inventoryItem.Quantity += quantity;
            else
                _context.InventoryItems.Add(new InventoryItem { UserId = userId, ItemName = itemName, Quantity = quantity });

            await _context.SaveChangesAsync();
            return OperationResult.Success("Ürün başarıyla satın alındı.");
        }


        public async Task<List<MarketItem>> GetAllMarketItemsAsync()
        {
            return await _context.MarketItems.ToListAsync();
        }



        public async Task<OperationResult> SellItemAsync(int userId, string itemName, int quantity)
        {
            var user = await _context.Users.FindAsync(userId);
            var item = await _context.MarketItems.FirstOrDefaultAsync(i => i.Name == itemName);
            var inventoryItem = await _context.InventoryItems
                .FirstOrDefaultAsync(i => i.UserId == userId && i.ItemName == itemName);

            if (user == null || item == null || inventoryItem == null)
                return OperationResult.Failure("Veri bulunamadı.");

            if (inventoryItem.Quantity < quantity)
                return OperationResult.Failure("Envanterde yeterli ürün yok.");

            inventoryItem.Quantity -= quantity;
            user.Coin += item.Price * quantity;
            item.Stock += quantity;

            if (inventoryItem.Quantity == 0)
                _context.InventoryItems.Remove(inventoryItem);

            await _context.SaveChangesAsync();
            return OperationResult.Success("Ürün başarıyla satıldı.");
        }
    }
}
