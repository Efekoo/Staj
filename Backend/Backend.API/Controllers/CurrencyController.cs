using Backend.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Backend.DataAccess.Contexts;



namespace Backend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurrencyController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CurrencyController(AppDbContext context)
        {
            _context = context;
        }

    }
}
