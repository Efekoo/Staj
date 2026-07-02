using System.Net.Http.Json;
using Backend.Core.DTOs;
using Microsoft.AspNetCore.SignalR.Client;

namespace Backend.Tests.IntegrationTests;

public class LeaderboardHubTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;

    public LeaderboardHubTests(ApiFactory factory) => _factory = factory;

    [Fact]
    public async Task AddXp_BroadcastsUpdatedLeaderboard_ToConnectedClients()
    {
        var client = _factory.CreateClient();
        var email = $"hub-{Guid.NewGuid():N}@test.com";
        await client.PostAsJsonAsync("/api/Auth/register",
            new { username = "hubuser", email, password = "Test123!" });
        var login = await client.PostAsJsonAsync("/api/Auth/login", new { email, password = "Test123!" });
        var token = System.Text.Json.JsonDocument.Parse(await login.Content.ReadAsStringAsync())
            .RootElement.GetProperty("token").GetString()!;
        client.DefaultRequestHeaders.Authorization = new("Bearer", token);

        await using var connection = new HubConnectionBuilder()
            .WithUrl(new Uri(_factory.Server.BaseAddress, "hubs/leaderboard"), options =>
            {
                options.HttpMessageHandlerFactory = _ => _factory.Server.CreateHandler();
            })
            .Build();

        var received = new TaskCompletionSource<List<UserLeaderboardDto>>();
        connection.On<List<UserLeaderboardDto>>("LeaderboardUpdated", standings => received.TrySetResult(standings));
        await connection.StartAsync();

        await client.PostAsJsonAsync("/api/User/add-xp", 50);

        var standings = await received.Task.WaitAsync(TimeSpan.FromSeconds(10));
        Assert.NotEmpty(standings);
        Assert.Equal(1, standings[0].Rank);
    }
}
