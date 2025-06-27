using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Core.Entities
{
    public class Score
    {
        public int Id { get; set; }
        public int Points { get; set; } = 0;
        public DateTime UpdatedAt { get; set; }
        public int PlayerId { get; set; }
        public Player Player { get; set; }
        public Score()
        {
            UpdatedAt = DateTime.UtcNow;
            Player = null;
        }
    }
}
