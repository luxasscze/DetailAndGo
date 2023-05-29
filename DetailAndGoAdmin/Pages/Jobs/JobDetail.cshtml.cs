using DetailAndGo.Models;
using DetailAndGo.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DetailAndGoAdmin.Pages.Jobs
{
    [Authorize]
    public class JobDetailModel : PageModel
    {
        private readonly IBookingService _bookingService;
        private readonly ICarService _carService;
        private readonly IDAGService _serviceService;
        private readonly IEmailService _emailService;

        public JobDetailModel(IBookingService bookingService, ICarService carService, IDAGService serviceService, IEmailService emailService)
        {
            _bookingService = bookingService;
            _carService = carService;
            _serviceService = serviceService;
            _emailService = emailService;
        }

        public Booking booking { get; set; }
        public Car? car { get; set; }
        public List<Service> services {get; set;}
        public List<Service> subServices { get; set; }

        public async Task OnGet(int id)
        {
            booking = await _bookingService.GetBookingById(id);
            car = await _carService.GetCarById(booking.CarId);
            services = await GetServicesFromArray(booking.ServicesArray);
            subServices = await _serviceService.GetServicesFromIdArray(booking.SubServicesArray);           
        }

        public async Task<List<Service>> GetServicesFromArray(string array)
        {
           return await _serviceService.GetServicesFromIdArray(array);
        }

        public async Task<ActionResult> OnGetDeclineBooking(int bookingId,string reason)
        {
            await _bookingService.DeclineBooking(bookingId, reason);
            Email email = new Email()
            {
                Body = "You sucks, your booking has been declined!<br /> Reason: <br /> " + reason,
                From = "admin@detailandgo.co.uk",
                IsHtml = true,
                Subject = "Detail&Go booking has been declined!",
                To = "lukas2slivka@gmail.com"
            };
            await _emailService.SendSingleEmail(email);
            return RedirectToPage("JobDetail", new { id = bookingId});
        }

        public async Task<ActionResult> OnGetReinstateBooking(int bookingId)
        {
            await _bookingService.ReinstateBooking(bookingId);
            return RedirectToPage("JobDetail", new { id = bookingId });
        }
    }
}
