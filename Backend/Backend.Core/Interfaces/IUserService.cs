using Backend.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Core.Interfaces
{
    public interface IUserService
    {
        Task<bool> RegisterAsync(User user, string plainPassword);
        Task<User?> LoginAsync(string email, string plainPassword);
        Task<bool> EmailExistsAsync(string email);
    }
}
