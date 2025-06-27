using Backend.API.Models.Dtos;
using Backend.Core.Entities;
using Backend.Core.Utilities;
using Backend.DataAccess.Context;
using System;
using System.Threading.Tasks;

namespace Backend.Business.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly Context _context;

        public PlayerService(Context context)
        {
            _context = context;
        }

        public async Task<OperationResult> UpdatePasswordAsync(int playerId, UpdatePasswordDto dto)
        {
            var player = await _context.Players.FindAsync(playerId);
            if (player == null)
                return OperationResult.Failure("Player not found.");

            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, player.PasswordHash))
                return OperationResult.Failure("Current password is incorrect.");

            if (BCrypt.Net.BCrypt.Verify(dto.NewPassword, player.PasswordHash))
                return OperationResult.Failure("New password cannot be the same as the current password.");

            player.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            await _context.SaveChangesAsync();

            return OperationResult.SuccessResult("Password updated successfully.");
        }

        public async Task<Player?> GetPlayerByIdAsync(int id)
        {
            return await _context.Players.FindAsync(id);
        }

        public async Task AddScoreAsync(int playerId, int score)
        {
            var player = await _context.Players.FindAsync(playerId);
            if (player == null) throw new Exception("Player not found.");

            player.Score += score;
            await _context.SaveChangesAsync();
        }
    }
}