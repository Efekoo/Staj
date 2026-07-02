using Backend.Business.Services;
using Backend.Core.Entities;
using Backend.DataAccess.Contexts;
using Backend.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Backend.Tests.UnitTests;

public class UserManagerTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static UserManager CreateManager(AppDbContext context)
        => new(new UserRepository(context), context);

    [Fact]
    public async Task RegisterAsync_HashesPassword_AndStoresUser()
    {
        using var context = CreateContext();
        var manager = CreateManager(context);
        var user = new User { Username = "efe", Email = "efe@test.com" };

        var result = await manager.RegisterAsync(user, "secret123");

        Assert.True(result);
        var stored = await context.Users.SingleAsync();
        Assert.NotEqual("secret123", stored.PasswordHash);
        Assert.True(BCrypt.Net.BCrypt.Verify("secret123", stored.PasswordHash));
    }

    [Fact]
    public async Task RegisterAsync_DuplicateEmail_ReturnsFalse()
    {
        using var context = CreateContext();
        var manager = CreateManager(context);
        await manager.RegisterAsync(new User { Username = "a", Email = "dup@test.com" }, "pw1");

        var result = await manager.RegisterAsync(new User { Username = "b", Email = "dup@test.com" }, "pw2");

        Assert.False(result);
        Assert.Equal(1, await context.Users.CountAsync());
    }

    [Fact]
    public async Task LoginAsync_CorrectPassword_ReturnsUser()
    {
        using var context = CreateContext();
        var manager = CreateManager(context);
        await manager.RegisterAsync(new User { Username = "efe", Email = "efe@test.com" }, "secret123");

        var user = await manager.LoginAsync("efe@test.com", "secret123");

        Assert.NotNull(user);
        Assert.Equal("efe", user.Username);
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_ReturnsNull()
    {
        using var context = CreateContext();
        var manager = CreateManager(context);
        await manager.RegisterAsync(new User { Username = "efe", Email = "efe@test.com" }, "secret123");

        var user = await manager.LoginAsync("efe@test.com", "wrong");

        Assert.Null(user);
    }

    [Fact]
    public async Task AddXPAsync_LevelsUp_AndGrantsCoins()
    {
        using var context = CreateContext();
        var manager = CreateManager(context);
        context.Users.Add(new User { Id = 1, Username = "efe", Email = "e@t.com", PasswordHash = "x" });
        await context.SaveChangesAsync();

        // Level 1 threshold is 100 XP: 250 XP => level 2 with 150 XP left, +100 coins
        await manager.AddXPAsync(1, 250);

        var user = await context.Users.SingleAsync();
        Assert.Equal(2, user.Level);
        Assert.Equal(150, user.XP);
        Assert.Equal(200, user.Coin);
    }

    [Fact]
    public async Task AddXPAsync_CanLevelUpMultipleTimes()
    {
        using var context = CreateContext();
        var manager = CreateManager(context);
        context.Users.Add(new User { Id = 1, Username = "efe", Email = "e@t.com", PasswordHash = "x" });
        await context.SaveChangesAsync();

        // 300 XP: level 1->2 consumes 100, level 2->3 consumes 200, 0 XP left
        await manager.AddXPAsync(1, 300);

        var user = await context.Users.SingleAsync();
        Assert.Equal(3, user.Level);
        Assert.Equal(0, user.XP);
        Assert.Equal(300, user.Coin);
    }
}
