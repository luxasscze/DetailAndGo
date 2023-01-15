using DetailAndGo.Models;
using DetailAndGo.Services;
using DetailAndGo.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DetailAndGo.Pages
{
    public class CreateBookingModel : PageModel
    {        
        private readonly ICustomerService _customerService;
        private readonly ICarService _carService;
        private readonly IBookingService _bookingService;
        private readonly IDAGService _serviceService;
        private readonly IStripeService _stripeService;

        public CreateBookingModel(ICustomerService customerService, ICarService carService, IBookingService bookingService, IDAGService serviceService, IStripeService stripeService)
        {
            _customerService = customerService;
            _carService = carService;
            _bookingService = bookingService;
            _serviceService = serviceService;
            _stripeService = stripeService;
        }

        public Customer Customer { get; set; }
        public Car CustomerCar { get; set; }
        public List<Car> CustomerCars { get; set; }
        public List<CarHistory> CarHistory { get; set; }
        public string AllBookings { get; set; }
        public List<string> SelectedServiceNames { get; set; }
        public List<Service> AllServices { get; set; }
        public CreateBooking CreateBooking { get; set; }        
        public string DefaultPaymentMethod { get; set; }
        public string Last4 { get; set; }

        public async Task OnGetAsync()
        {
            Customer = _customerService.GetCustomerByEmail(User.Identity.Name);
            CustomerCar = await _carService.GetCustomerActiveCar(Customer.AspNetUserId);
            CustomerCars = await _carService.GetCustomerCars(Customer.AspNetUserId);
            CarHistory = await _carService.GetCarHistoryByCarId(CustomerCar.Id);

            AllBookings = "";
            SelectedServiceNames = new List<string>();
            AllServices = new List<Service>();
            CreateBooking = new CreateBooking();
            DefaultPaymentMethod = "";
            Last4 = _stripeService.GetLast4(Customer.StripeId);
        }
    }
}
