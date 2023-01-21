using DetailAndGo.Data;
using DetailAndGo.Models;
using DetailAndGo.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DetailAndGo.Services
{
    public class DAGService : IDAGService
    {
        public ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DAGService(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<Service>> GetAllServices()
        {
            List<Service> allServices = await _context.Services.ToListAsync();
            return allServices;
        }
        
        public async Task<List<Service>> GetAllSubServices()
        {
            return await _context.Services.Where(s => s.IsSubService).ToListAsync();
        }
    }
}
