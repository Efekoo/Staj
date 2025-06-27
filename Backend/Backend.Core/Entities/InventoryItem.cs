using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Core.Entities
{
    public class InventoryItem
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int Stock { get; set; } = 1;
        public DateTime AcquiredAt { get; set; }

        public Player Player { get; set; }

        public InventoryItem()
        {
            AcquiredAt = DateTime.UtcNow;
            Player = null; 
        }
    }
}
