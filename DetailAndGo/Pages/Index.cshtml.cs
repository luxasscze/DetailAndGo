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

        public IndexModel(ILogger<IndexModel> logger, ICustomerService customerService, ICarService carService)
        {
            _logger = logger;
            _customerService = customerService;
            _carService = carService;
        }

        public Customer Customer { get; set; }
        public Car CustomerCar { get; set; }
        public List<Car> CustomerCars { get; set; }

        public void OnGetAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                Customer = _customerService.GetCustomerByEmail(User.Identity.Name);
                CustomerCar = _carService.GetCustomerActiveCar(Customer.AspNetUserId).Result;
                CustomerCars = _carService.GetCustomerCars(Customer.AspNetUserId).Result;
                if(CustomerCar == null)
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
            }
        }

        public async Task<ActionResult> OnPostMakeCarActiveAsync(int carIndex)
        {
            string aspNetUserId = _customerService.GetCustomerByEmail(User.Identity.Name).AspNetUserId;
            Car car = await _carService.GetCarByIndex(aspNetUserId, carIndex);
            await _carService.MakeCarActive(car.Id, aspNetUserId);
            return RedirectToAction("Get");
        }
    }
}