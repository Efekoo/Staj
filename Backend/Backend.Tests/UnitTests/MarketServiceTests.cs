using Backend.Business.Services;
using Backend.Core.Entities;
using Backend.DataAccess.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Backend.Tests.UnitTests;

public class MarketServiceTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var context = new AppDbContext(options);
        context.Users.Add(new User { Id = 1, Username = "efe", Email = "e@t.com", PasswordHash = "x", Coin = 100 });
        context.MarketItems.Add(new MarketItem { Id = 1, Name = "Elma", Price = 10, Stock = 5 });
        context.SaveChanges();
        return context;
    }

    [Fact]
    public async Task BuyItemAsync_DecreasesStockAndCoins_AddsToInventory()
    {
        using var context = CreateContext();
        var service = new MarketService(context);

        var result = await service.BuyItemAsync(1, "Elma", 2);

        Assert.True(result.IsSuccess);
        Assert.Equal(3, (await context.MarketItems.SingleAsync()).Stock);
        Assert.Equal(80, (await context.Users.SingleAsync()).Coin);
        var inventory = await context.InventoryItems.SingleAsync();
        Assert.Equal("Elma", inventory.ItemName);
        Assert.Equal(2, inventory.Quantity);
    }

    [Fact]
    public async Task BuyItemAsync_InsufficientStock_Fails()
    {
        using var context = CreateContext();
        var service = new MarketService(context);

        var result = await service.BuyItemAsync(1, "Elma", 99);

        Assert.False(result.IsSuccess);
        Assert.Equal(5, (await context.MarketItems.SingleAsync()).Stock);
    }

    [Fact]
    public async Task BuyItemAsync_InsufficientCoins_Fails()
    {
        using var context = CreateContext();
        context.MarketItems.Add(new MarketItem { Id = 2, Name = "Zırh", Price = 150, Stock = 8 });
        await context.SaveChangesAsync();
        var service = new MarketService(context);

        var result = await service.BuyItemAsync(1, "Zırh", 1);

        Assert.False(result.IsSuccess);
        Assert.Equal(100, (await context.Users.SingleAsync()).Coin);
    }

    [Fact]
    public async Task SellItemAsync_AddsCoins_AndRemovesEmptyInventoryEntry()
    {
        using var context = CreateContext();
        context.InventoryItems.Add(new InventoryItem { UserId = 1, ItemName = "Elma", Quantity = 2 });
        await context.SaveChangesAsync();
        var service = new MarketService(context);

        var result = await service.SellItemAsync(1, "Elma", 2);

        Assert.True(result.IsSuccess);
        Assert.Equal(120, (await context.Users.SingleAsync()).Coin);
        Assert.Equal(7, (await context.MarketItems.SingleAsync()).Stock);
        Assert.Empty(context.InventoryItems);
    }

    [Fact]
    public async Task SellItemAsync_ItemNotInInventory_Fails()
    {
        using var context = CreateContext();
        var service = new MarketService(context);

        var result = await service.SellItemAsync(1, "Elma", 1);

        Assert.False(result.IsSuccess);
        Assert.Equal(100, (await context.Users.SingleAsync()).Coin);
    }
}
