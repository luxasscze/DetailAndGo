using DetailAndGo.Data;
using DetailAndGo.Models;
using DetailAndGo.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace DetailAndGo.Services
{
    public class CustomerService : ICustomerService
    {
        
        public ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CustomerService(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            
                _context = context;
            _userManager = userManager;
            
        }

        public Customer GetCustomerById(string? customerId)
        {
            return _context.Customers.Where(s => s.AspNetUserId == customerId).FirstOrDefault();
        }

        public async Task<bool> RegisterCustomerAsync(Customer customer)
        {
            if (customer != null)
            {
                await _context.Customers.AddAsync(customer);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public bool CheckEmailExists(string email)
        {
            Customer customer = _context.Customers.Where(s => s.Email == email).FirstOrDefault();            
            IdentityUser user = _userManager.Users.Where(s => s.Email == email).FirstOrDefault();
            
            return customer != null || user != null ? true : false;
        }
    }
}
