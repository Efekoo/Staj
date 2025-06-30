using Backend.Core.Entities;


namespace Backend.Core.Interfaces
{
    public interface IUserService
    {
        Task<bool> RegisterAsync(User user, string plainPassword);
        Task<User?> LoginAsync(string email, string plainPassword);
        Task<bool> EmailExistsAsync(string email);
    }
}
