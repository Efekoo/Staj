using Backend.Core.Entities;
using Backend.Core.DTOs;


namespace Backend.Core.Interfaces
{
    public interface IUserService
    {
        Task<bool> RegisterAsync(User user, string plainPassword);
        Task<User?> LoginAsync(string email, string plainPassword);
        Task<bool> EmailExistsAsync(string email);
        Task AddXPAsync(int userId, int xpToAdd);
        Task<User?> GetUserByIdAsync(int id);
        Task<List<UserLeaderboardDto>> GetTopUsersAsync();

    }
}
