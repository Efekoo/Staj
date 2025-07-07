using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Business.Dtos
{
    public class UserLeaderboardDto
    {
     public string Username { get; set; } = null!;
     public int Level { get; set; }
     public int XP { get; set; }
    }
}
