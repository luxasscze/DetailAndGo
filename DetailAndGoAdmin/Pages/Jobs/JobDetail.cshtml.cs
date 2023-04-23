using DetailAndGo.Models;
using DetailAndGo.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DetailAndGoAdmin.Pages.Jobs
{
    public class JobDetailModel : PageModel
    {        
        private readonly IBookingService _bookingService;

        public JobDetailModel(IBookingService bookingService)
        {            
            _bookingService = bookingService;
        }

        public Booking booking { get; set; }

        public async Task OnGet(int id)
        {
            booking = await _bookingService.GetBookingById(id);
        }
    }
}
