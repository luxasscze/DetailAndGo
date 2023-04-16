using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DetailAndGo.Models;
using DetailAndGoAdmin.Data;
using DetailAndGo.Enums;
using Microsoft.AspNetCore.Authorization;
using DetailAndGo.Services.Interfaces;

namespace DetailAndGoAdmin.Pages.Jobs
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly DetailAndGo.Data.ApplicationDbContext _context;
        private IBookingService _bookingService;
        private ICustomerService _customerService;

        public IndexModel(DetailAndGo.Data.ApplicationDbContext context, IBookingService bookingService, ICustomerService customerService)
        {
            _context = context;
            _bookingService = bookingService;
            _customerService = customerService;
        }

        public IList<Booking> Bookings { get;set; }

        public async Task OnGetAsync()
        {
            if (_context.Jobs != null)
            {
                Bookings = await _bookingService.GetAllCreatedBookings();               
            }           
            
        }

        public JsonResult OnGetGetUserFullName(string aspNetUserId)
        {
            Customer customer = _customerService.GetCustomerById(aspNetUserId);
            return new JsonResult(customer.FirstName + " " + customer.LastName);
        }
    }
}
