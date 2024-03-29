﻿using DetailAndGo.Data;
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

        public Customer GetCustomerByEmail(string? email)
        {
            return _context.Customers.Where(s => s.Email == email).FirstOrDefault();
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

        /// <summary>
        /// Function to check if email exists in DB[AspNetUsers or Customers]
        /// </summary>
        /// <param name="email">email input to be checked</param>
        /// <returns>Returns true if email does exists</returns>
        public async Task<bool> CheckEmailExists(string email)
        {
            Customer customer = await _context.Customers.Where(s => s.Email == email).FirstOrDefaultAsync();            
            IdentityUser user = await _userManager.Users.Where(s => s.Email == email).FirstOrDefaultAsync();
            
            return customer != null || user != null ? true : false;
        }
    }
}
