﻿using DetailAndGo.Models;
using DetailAndGo.Services;
using DetailAndGo.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using NuGet.Packaging;
using Stripe;

namespace DetailAndGo.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ICustomerService _customerService;
        private readonly ICarService _carService;
        private readonly IBookingService _bookingService;
        private readonly IDAGService _serviceService;
        private readonly IStripeService _stripeService;
        private readonly IEmailService _emailService;

        public IndexModel(ILogger<IndexModel> logger, ICustomerService customerService, ICarService carService, IBookingService bookingService, IDAGService serviceService, IStripeService stripeService, IEmailService emailService)
        {
            _logger = logger;
            _customerService = customerService;
            _carService = carService;
            _bookingService = bookingService;
            _serviceService = serviceService;
            _stripeService = stripeService;
            _emailService = emailService;
        }

        public DetailAndGo.Models.Customer Customer { get; set; }
        public Car CustomerCar { get; set; }
        public List<Car> CustomerCars { get; set; }
        public List<CarHistory> CarHistory { get; set; }
        public string AllBookings { get; set; }
        public List<string> SelectedServiceNames { get; set; }
        public List<Service> AllServices { get; set; }
        public CreateBooking CreateBooking { get; set; }
        public string Greeting { get; set; }
        public string DefaultPaymentMethod { get; set; }
        public string Last4 { get; set; }
        public Booking ActiveBooking { get; set; }

        public async Task OnGetAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                GetGreeting();
                AllBookings = _bookingService.GetAllActiveBookingsAsCalendarEvents().Result;
                Customer = _customerService.GetCustomerByEmail(User.Identity.Name);
                CustomerCar = _carService.GetCustomerActiveCar(Customer.AspNetUserId).Result;
                CustomerCars = _carService.GetCustomerCars(Customer.AspNetUserId).Result;
                CarHistory = _carService.GetCarHistoryByCarId(CustomerCar.Id).Result.Where(s => s.BookingDate < DateTime.Now).OrderByDescending(s => s.BookingDate).Take(3).ToList();
                AllServices = _serviceService.GetAllServices().Result.Where(s => s.IsActive == true).ToList();
                ActiveBooking = await _bookingService.GetCustomerActiveBooking(Customer.AspNetUserId);

                string stripeId = _customerService.GetCustomerByEmail(User.Identity.Name).StripeId;
                DefaultPaymentMethod = _stripeService.GetCustomerDefaultPaymentMethod(stripeId);
                Last4 = _stripeService.GetLast4(stripeId);

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

        public void GetGreeting()
        {
            if (DateTime.Now.Hour > 6 && DateTime.Now.Hour <= 12)
            {
                Greeting = "Good morning";
            }
            else if (DateTime.Now.Hour > 12 && DateTime.Now.Hour <= 17)
            {
                Greeting = "Good afternoon";
            }
            else if (DateTime.Now.Hour > 17 && DateTime.Now.Hour < 23)
            {
                Greeting = "Good evening";
            }
            else
            {
                Greeting = "Night night";
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
        public async Task<ActionResult> OnPostCreateBookingAsync(int carIndex, string services, DateTime bookedFor, string paymentMethodId)
        {
            Customer = _customerService.GetCustomerByEmail(User.Identity.Name);
            Booking booking = new Booking()
            {
                AspNetUserId = Customer.AspNetUserId,
                BookedFor = bookedFor,
                CarId = _carService.GetCarByIndex(Customer.AspNetUserId, carIndex).Result.Id,
                Created = DateTime.Now,
                Image = "",
                Notes = "",
                PaymentMethodId = paymentMethodId,
                Services = new List<Service>(),
                Status = Models.Enums.BookingStatus.Created
            };
            //await _bookingService.CreateBooking(booking);
            return RedirectToAction("Get");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> OnPostGetAllBookingsAsync()
        {
            await _bookingService.GetAllActiveBookinsAsJSON();
            return RedirectToAction("Get");
        }


        [HttpGet]
        public async Task<JsonResult> OnGetServiceNamesAsync(string sServices)
        {
            List<string> selServices = sServices.Split(',').ToList();
            string serviceName = string.Empty;
            List<Service> services = await _serviceService.GetAllServices();
            List<string> result = new List<string>();
            foreach (string serviceId in selServices)
            {
                result.Add(services.FirstOrDefault(s => s.Id == Convert.ToInt32(serviceId)).Name);
            }
            SelectedServiceNames = result;
            return new JsonResult(result);
        }

        [HttpGet]
        public async Task<JsonResult> OnGetPaymentMethodsAsync()
        {
            string stripeId = _customerService.GetCustomerByEmail(User.Identity.Name).StripeId;
            StripeList<PaymentMethod> paymentMethods = await _stripeService.GetCustomerPaymentMethods(stripeId);
            return new JsonResult(paymentMethods);
        }

        [HttpGet]
        public async Task<JsonResult> OnGetCancelBooking(int bookingId)
        {
            await _bookingService.CancelBooking(bookingId, "Booking has been cancelled");
            Email email = new Email()
            {
                Body = "You cancelled your booking #" + bookingId,
                From = "admin@detailandgo.co.uk",
                IsHtml = true,
                Subject = "Detail&Go - Booking #" + bookingId + " has been cancelled",
                To = User.Identity.Name
            };
            await _emailService.SendSingleEmail(email);
            return new JsonResult(true);
        }

        [HttpGet]
        public async Task<JsonResult> OnGetSendTestingEmail()
        {
            Email email = new Email()
            {
                Body = "Babe, I want to fuck you!",
                From = "admin@detailandgo.co.uk",
                IsHtml = true,
                Subject = "To my love!",
                To = "lukas2slivka@gmail.com"
            };
            await _emailService.SendSingleEmail(email);
            return new JsonResult(true);
        }

        /*[HttpPost]
        public async Task<ActionResult> OnPostChargeCustomerAsync(double amount)
        {
            Models.Customer customer = _customerService.GetCustomerByEmail(User.Identity.Name);
            long amountToCharge = (long)amount;
            var test = await _stripeService.ChargeCustomerForBooking(customer, amountToCharge);

            return RedirectToAction("Get");
        }*/

        [HttpPost]
        public async Task<ActionResult> OnPostChangePaymentMethodAsync(string paymentMethodId)
        {
            string stripeId = _customerService.GetCustomerByEmail(User.Identity.Name).StripeId;
            await _stripeService.SetCustomerDefaultPaymentMethod(stripeId, paymentMethodId);
            return RedirectToAction("Get");
        }

        [HttpPost]
        public async Task<ActionResult> OnPostRemovePaymentMethodAsync(string paymentMethodId)
        {
            string stripeId = _customerService.GetCustomerByEmail(User.Identity.Name).StripeId;
            await _stripeService.RemovePaymentMethod(stripeId, paymentMethodId);
            return RedirectToAction("Get");
        }
    }
}