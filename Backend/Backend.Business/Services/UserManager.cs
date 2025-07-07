using Backend.Core.DTOs;
using Backend.Core.Entities;
using Backend.Core.Interfaces;
using Backend.DataAccess.Contexts;
using Backend.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Backend.Business.Services
{

    public class UserManager : IUserService
    {

        private readonly UserRepository _userRepository;
        private readonly AppDbContext _context;

        public UserManager(UserRepository userRepository, AppDbContext context)
        {
            _userRepository = userRepository;
            _context = context;

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

        public async Task AddXPAsync (int Id, int xpToAdd)
        {
            var user = await _userRepository.GetUserByIdAsync(Id);
            if (user == null) return;
            
            user.XP += xpToAdd;

            while (user.XP>= user.Level * 100)
            {
                user.XP -= user.Level * 100;
                user.Level++;
                user.Coin += 100;
            }
            await _userRepository.UpdateUserAsync(user);


        }
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }
        public async Task<List<UserLeaderboardDto>> GetTopUsersAsync()
        {
            var user=await _context.Users
                .OrderByDescending(u => u.Level)
                .ThenByDescending(u => u.XP)
                .Take(10)
                .Select(u => new UserLeaderboardDto
                {
                    Username = u.Username,
                    Level = u.Level,
                    XP = u.XP
                })
                .ToListAsync();

            for (int i = 0; i < user.Count; i++)
            {
                user[i].Rank = i + 1;
            }
            return user;
        }
    }
}
