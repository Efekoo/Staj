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
        public int PlayerId { get; set; }
        public User User { get; set; } = null!;

        public string ItemName { get; set; } = null!;
        public int Quantity { get; set; }
    }
}
