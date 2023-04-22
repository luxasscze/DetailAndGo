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

        public async Task<List<Service>> GetAllServicesOrderedByCreated()
        {
            List<Service> services = await _context.Services.OrderByDescending(s => s.CreatedDate).ToListAsync();
            return services;
        }
        
        public async Task<List<Service>> GetAllSubServices()
        {
            return await _context.Services.Where(s => s.IsSubService).ToListAsync();
        }

        public async Task<Service> GetServiceById(int serviceId)
        {
            return await _context.Services.FirstOrDefaultAsync(s => s.Id == serviceId);
        }

        public async Task<List<Service>> GetAllMainServices()
        {
            return await _context.Services.Where(s => !s.IsSubService && s.IsActive).ToListAsync();
        }

        public async Task<List<Service>> GetServicesFromIdArray(string Ids)
        {
            List<Service> services = new List<Service>();
            string[] ids = Ids.Split(',');

            foreach(string id in ids)
            {
                services.Add(await _context.Services.FirstOrDefaultAsync(s => s.Id == Convert.ToInt32(id)));
            }

            return services;
        }

        public string GetServiceNameById(int id)
        {
            if(_context.Services.FirstOrDefault(x => x.Id == id) == null)
            {
                return "";
            }
            return _context.Services.FirstOrDefault(x => x.Id == id).Name;
        }

        public string GetServiceSuperDescription(int id)
        {
            return _context.Services.FirstOrDefault(x => x.Id == id).SuperDescription;
        }

        public decimal GetServicePrice(int id)
        {
            return _context.Services.FirstOrDefault(s => s.Id == id).Price;
        }
    }
}
