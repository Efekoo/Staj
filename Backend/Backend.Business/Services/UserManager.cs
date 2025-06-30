using Backend.Core.Entities;
using Backend.Core.Interfaces;
using Backend.DataAccess.Repositories;
using BCrypt.Net;

namespace Backend.Business.Services
{

    public class UserManager : IUserService
    {
        private readonly UserRepository _userRepository;

        public UserManager(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> RegisterAsync(User user, string plainPassword)
        {
            if (await EmailExistsAsync(user.Email))
                return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword);
            await _userRepository.AddUserAsync(user);
            return true;
        }

        public async Task<User?> LoginAsync(string email, string plainPassword)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return null;

            bool isValid = BCrypt.Net.BCrypt.Verify(plainPassword, user.PasswordHash);
            return isValid ? user : null;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return user != null;
        }
    }
}
