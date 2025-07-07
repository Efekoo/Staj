using Backend.Core.Entities;
using Backend.DataAccess.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Backend.DataAccess.Repositories
{
    public class FriendRepository
    {
        private readonly AppDbContext _context;

        public FriendRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task SendFriendRequestAsync(FriendRequest request)
        {
            _context.FriendRequests.Add(request);
            await _context.SaveChangesAsync();
        }

        public async Task<List<FriendRequest>> GetPendingRequestsAsync(int userId)
        {
            return await _context.FriendRequests
                .Where(fr => fr.ReceiverId == userId && !fr.IsAccepted)
                .Include(fr => fr.Sender)
                .ToListAsync();
        }

        public async Task AcceptFriendRequestAsync(int senderId, int receiverId)
        {
            var request = await _context.FriendRequests
                .FirstOrDefaultAsync(fr => fr.SenderId == senderId && fr.ReceiverId == receiverId && !fr.IsAccepted);

            if (request != null)
            {
                request.IsAccepted = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<User>> GetFriendsAsync(int userId)
        {
            var accepted = await _context.FriendRequests
                .Where(fr => (fr.SenderId == userId || fr.ReceiverId == userId) && fr.IsAccepted)
                .Include(fr => fr.Sender)
                .Include(fr => fr.Receiver)
                .ToListAsync();

            return accepted
                .Select(fr => fr.SenderId == userId ? fr.Receiver : fr.Sender)
                .ToList();
        }
    }
}
