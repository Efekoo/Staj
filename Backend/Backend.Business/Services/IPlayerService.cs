using Backend.API.Models.Dtos;
using Backend.Core.Entities;
using Backend.Core.Utilities;
using System.Threading.Tasks;

namespace Backend.Business.Services
{
    public interface IPlayerService
    {
        Task<OperationResult> UpdatePasswordAsync(int playerId, UpdatePasswordDto dto);
        Task<Player?> GetPlayerByIdAsync(int id);
        Task AddScoreAsync(int playerId, int score);
    }
}