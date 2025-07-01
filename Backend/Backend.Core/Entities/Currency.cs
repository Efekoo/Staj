using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Core.Entities
{
     public class Currency
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public User User { get; set; } = null!;

        public int Coin { get; set; }
    }
}
