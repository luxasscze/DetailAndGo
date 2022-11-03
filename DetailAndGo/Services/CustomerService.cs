using DetailAndGo.Data;
using DetailAndGo.Models;
using DetailAndGo.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DetailAndGo.Services
{
    public class CustomerService : ICustomerService
    {
        
        public ApplicationDbContext _context;

        public CustomerService(ApplicationDbContext context)
        {
            
                _context = context;
            
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
    }
}
