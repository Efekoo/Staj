using Microsoft.AspNetCore.SignalR;

namespace Backend.API.Hubs;

/// <summary>
/// Clients subscribe to receive "LeaderboardUpdated" events with the
/// current top-10 standings whenever any player's XP changes.
/// </summary>
public class LeaderboardHub : Hub
{
}
