using DetailAndGo.Models;
using DetailAndGo.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DetailAndGoAdmin.Pages.Jobs
{
    public class JobDetailModel : PageModel
    {
        private readonly IBookingService _bookingService;
        private readonly ICarService _carService;
        private readonly IDAGService _serviceService;

        public JobDetailModel(IBookingService bookingService, ICarService carService, IDAGService serviceService)
        {
            _bookingService = bookingService;
            _carService = carService;
            _serviceService = serviceService;
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
            subServices = subServices.Where((number, index) => index % 2 == 1).ToList();

        }

        public async Task<List<Service>> GetServicesFromArray(string array)
        {
           return await _serviceService.GetServicesFromIdArray(array);
        }
    }
}
