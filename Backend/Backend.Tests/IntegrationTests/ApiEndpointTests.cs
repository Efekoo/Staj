using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Backend.Tests.IntegrationTests;

public class ApiEndpointTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;

    public ApiEndpointTests(ApiFactory factory) => _factory = factory;

    private static string UniqueEmail() => $"user-{Guid.NewGuid():N}@test.com";

    private static async Task<string> RegisterAndLoginAsync(HttpClient client, string? email = null)
    {
        email ??= UniqueEmail();
        var register = await client.PostAsJsonAsync("/api/Auth/register",
            new { username = email.Split('@')[0], email, password = "Test123!" });
        register.EnsureSuccessStatusCode();

        var login = await client.PostAsJsonAsync("/api/Auth/login", new { email, password = "Test123!" });
        login.EnsureSuccessStatusCode();

        var json = JsonDocument.Parse(await login.Content.ReadAsStringAsync());
        return json.RootElement.GetProperty("token").GetString()!;
    }

    private static HttpClient WithToken(HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    [Fact]
    public async Task Register_ReturnsOk()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/Auth/register",
            new { username = "newuser", email = UniqueEmail(), password = "Test123!" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsBadRequest()
    {
        var client = _factory.CreateClient();
        var email = UniqueEmail();
        await client.PostAsJsonAsync("/api/Auth/register", new { username = "a", email, password = "pw" });

        var second = await client.PostAsJsonAsync("/api/Auth/register", new { username = "b", email, password = "pw" });

        Assert.Equal(HttpStatusCode.BadRequest, second.StatusCode);
    }

    [Fact]
    public async Task Login_WrongPassword_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();
        var email = UniqueEmail();
        await client.PostAsJsonAsync("/api/Auth/register", new { username = "a", email, password = "correct" });

        var login = await client.PostAsJsonAsync("/api/Auth/login", new { email, password = "wrong" });

        Assert.Equal(HttpStatusCode.Unauthorized, login.StatusCode);
    }

    [Fact]
    public async Task Login_ReturnsJwtToken()
    {
        var client = _factory.CreateClient();
        var token = await RegisterAndLoginAsync(client);

        Assert.Equal(3, token.Split('.').Length); // header.payload.signature
    }

    [Fact]
    public async Task UserMe_WithoutToken_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/User/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UserMe_WithToken_ReturnsProfileWithStartingCoins()
    {
        var client = _factory.CreateClient();
        var token = await RegisterAndLoginAsync(client);

        var response = await WithToken(client, token).GetAsync("/api/User/me");
        response.EnsureSuccessStatusCode();

        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(100, json.RootElement.GetProperty("coin").GetInt32());
        Assert.Equal(1, json.RootElement.GetProperty("level").GetInt32());
    }

    [Fact]
    public async Task Market_WithoutToken_ReturnsUnauthorized()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/Market");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Market_List_ContainsSeededItems()
    {
        var client = _factory.CreateClient();
        var token = await RegisterAndLoginAsync(client);

        var response = await WithToken(client, token).GetAsync("/api/Market");
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("Elma", body);
    }

    [Fact]
    public async Task Market_Buy_Succeeds_AndReducesCoins()
    {
        var client = _factory.CreateClient();
        var token = await RegisterAndLoginAsync(client);
        WithToken(client, token);

        var buy = await client.PostAsync("/api/Market/buy/Elma/1", null);
        Assert.Equal(HttpStatusCode.OK, buy.StatusCode);

        var me = JsonDocument.Parse(await client.GetStringAsync("/api/User/me"));
        Assert.Equal(90, me.RootElement.GetProperty("coin").GetInt32());
    }

    [Fact]
    public async Task Market_Buy_InsufficientBalance_ReturnsBadRequest()
    {
        var client = _factory.CreateClient();
        var token = await RegisterAndLoginAsync(client);
        WithToken(client, token);

        // Starting balance is 100 coins; Zırh costs 150
        var buy = await client.PostAsync("/api/Market/buy/Z%C4%B1rh/1", null);

        Assert.Equal(HttpStatusCode.BadRequest, buy.StatusCode);
    }

    [Fact]
    public async Task AddXp_LevelsUpUser()
    {
        var client = _factory.CreateClient();
        var token = await RegisterAndLoginAsync(client);
        WithToken(client, token);

        var response = await client.PostAsJsonAsync("/api/User/add-xp", 250);
        response.EnsureSuccessStatusCode();

        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(2, json.RootElement.GetProperty("level").GetInt32());
        Assert.Equal(150, json.RootElement.GetProperty("xp").GetInt32());
    }

    [Fact]
    public async Task Leaderboard_ReturnsRankedUsers()
    {
        var client = _factory.CreateClient();
        var token = await RegisterAndLoginAsync(client);

        var response = await WithToken(client, token).GetAsync("/api/Leaderboard");
        response.EnsureSuccessStatusCode();

        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.True(json.RootElement.GetArrayLength() >= 1);
        Assert.Equal(1, json.RootElement[0].GetProperty("rank").GetInt32());
    }
}
