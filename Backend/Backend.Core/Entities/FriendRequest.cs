using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Core.Entities
{
    public class FriendRequest
    {
        public int Id { get; set; }

        public int SenderId { get; set; }
        public int ReceiverId { get; set; }

        public bool IsAccepted { get; set; } = false;

        public DateTime CreatedAt { get; set; }

        public FriendRequest()
        {
            CreatedAt = DateTime.UtcNow;
        }
    }
}
