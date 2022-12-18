using DetailAndGo.Models;
using DetailAndGo.Services;
using DetailAndGo.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DetailAndGo.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ICustomerService _customerService;
        private readonly ICarService _carService;
        private readonly IBookingService _bookingService;
        private readonly IDAGService _serviceService;

        public IndexModel(ILogger<IndexModel> logger, ICustomerService customerService, ICarService carService, IBookingService bookingService, IDAGService serviceService)
        {
            _logger = logger;
            _customerService = customerService;
            _carService = carService;
            _bookingService = bookingService;
            _serviceService = serviceService;
        }

        public Customer Customer { get; set; }
        public Car CustomerCar { get; set; }
        public List<Car> CustomerCars { get; set; }
        public List<CarHistory> CarHistory { get; set; }
        public string AllBookings { get; set; }
        public List<Service> AllServices { get; set; }

        public void OnGetAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                AllBookings = _bookingService.GetAllActiveBookingsAsCalendarEvents().Result;                
                Customer = _customerService.GetCustomerByEmail(User.Identity.Name);
                CustomerCar = _carService.GetCustomerActiveCar(Customer.AspNetUserId).Result;
                CustomerCars = _carService.GetCustomerCars(Customer.AspNetUserId).Result;
                CarHistory = _carService.GetCarHistoryByCarId(CustomerCar.Id).Result.Where(s => s.BookingDate < DateTime.Now).OrderByDescending(s => s.BookingDate).Take(3).ToList();
                AllServices = _serviceService.GetAllServices().Result.Where(s => s.IsActive == true).ToList();
                if (CustomerCar == null)
                {
                    CustomerCar = new Car()
                    {
                        AspNetUserId = Customer.AspNetUserId,
                        CarModel = "No",
                        CarFamily = "Car",
                        Created = DateTime.Now,
                        IsPrimary = false,
                        Notes = "nocar"
                    };
                }
                if (CarHistory == null)
                {
                    CarHistory = new List<CarHistory>()
                    {
                        new CarHistory()
                        {
                            BookingDate = DateTime.Now,
                            CarId = 0,
                            Notes = "",
                            Status = "na",
                            Service = "no future booking"
                        }
                    };
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> OnPostMakeCarActiveAsync(int carIndex)
        {
            string aspNetUserId = _customerService.GetCustomerByEmail(User.Identity.Name).AspNetUserId;
            Car car = await _carService.GetCarByIndex(aspNetUserId, carIndex);
            await _carService.MakeCarActive(car.Id, aspNetUserId);
            return RedirectToAction("Get");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> OnPostAddNewCarAsync(string carModel, string carFamily)
        {
            string aspNetUserId = _customerService.GetCustomerByEmail(User.Identity.Name).AspNetUserId;
            Car car = new Car()
            {
                AspNetUserId = aspNetUserId,
                CarFamily = carFamily,
                CarModel = carModel,
                Created = DateTime.Now,
                IsPrimary = false,
                Notes = string.Empty
            };

            await _carService.SaveCar(car);
            Car latestCar = _carService.GetCustomerCars(aspNetUserId).Result.Last();
            await _carService.MakeCarActive(latestCar.Id, aspNetUserId);
            return RedirectToAction("Get");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> OnPostRemoveCarAsync(int carIndex)
        {
            string aspNetUserId = _customerService.GetCustomerByEmail(User.Identity.Name).AspNetUserId;
            Car car = await _carService.GetCarByIndex(aspNetUserId, carIndex);

            if (car.IsPrimary)
            {
                return RedirectToPage("Index", new { Message = "CantRemoveActiveCar" });
            }
            else
            {
                await _carService.RemoveCar(car);
                return RedirectToAction("Get");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> OnPostCreateBookingAsync(Booking booking)
        {
            await _bookingService.CreateBooking(booking);
            return RedirectToAction("Get");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> OnPostGetAllBookingsAsync()
        {
            await _bookingService.GetAllActiveBookinsAsJSON();
            return RedirectToAction("Get");
        }
    }
}