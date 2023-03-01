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
        private readonly IJobService _jobService;

        public CreateBookingModel(ICustomerService customerService, ICarService carService, IBookingService bookingService, IDAGService serviceService, IStripeService stripeService, IJobService jobService)
        {
            _customerService = customerService;
            _carService = carService;
            _bookingService = bookingService;
            _serviceService = serviceService;
            _stripeService = stripeService;
            _jobService = jobService;
        }

        public Customer Customer { get; set; }
        public Car CustomerCar { get; set; }
        public List<Car> CustomerCars { get; set; }
        public List<CarHistory> CarHistory { get; set; }
        public string AllBookings { get; set; }
        public List<string> SelectedServiceNames { get; set; }
        public List<Service> AllServices { get; set; }
        public List<Service> AllMainServices { get; set; }
        public List<Service> AllSubServices { get; set; }
        public CreateBooking CreateBooking { get; set; }        
        public string DefaultPaymentMethod { get; set; }
        public string Last4 { get; set; }
        public List<int> AvailableTimes { get; set; }
        public List<string> AvailableTimesString { get; set; }

        public async Task OnGetAsync()
        {
            Customer = _customerService.GetCustomerByEmail(User.Identity.Name);
            CustomerCar = await _carService.GetCustomerActiveCar(Customer.AspNetUserId);
            CustomerCars = await _carService.GetCustomerCars(Customer.AspNetUserId);
            CarHistory = await _carService.GetCarHistoryByCarId(CustomerCar.Id);
            AllMainServices = await _serviceService.GetAllMainServices();

            AllBookings = "";
            AllSubServices = await _serviceService.GetAllSubServices();
            SelectedServiceNames = new List<string>();
            AllServices = await _serviceService.GetAllServices();
            CreateBooking = new CreateBooking();
            DefaultPaymentMethod = _stripeService.GetCustomerDefaultPaymentMethod(Customer.StripeId);
            Last4 = _stripeService.GetLast4(Customer.StripeId);
            AvailableTimes = await _jobService.GetAvailableTimesForDate(DateTime.Now);
            AvailableTimesString = new List<string>();

            foreach(var item in AvailableTimes)
            {
                AvailableTimesString.Add(_jobService.ConvertTimeFromIntToString(item));
            }
        }

        
        public async Task<ActionResult> OnGetSetAvailableTimesByDateAsync(DateTime date)
        {           
            List<int> modelInt = await _jobService.GetAvailableTimesForDate(date);
            List<string> modelString = new List<string>();
            foreach(var item in modelInt)
            {
                modelString.Add(_jobService.ConvertTimeFromIntToString(item));
            }

            var test = modelString;

            return Partial("Partials/Times", modelString);
        }

     
        public async Task<JsonResult> OnGetGetServiceNameAsync(string service)
        {
            return new JsonResult(_serviceService.GetServiceNameById(int.Parse(service)));
        }

        public async Task<JsonResult> OnGetGetServiceSuperDescriptionTextAsync(string service)
        {
            return new JsonResult(_serviceService.GetServiceSuperDescription(int.Parse(service)));
        }
    }
}
