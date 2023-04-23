using DetailAndGo.Data;
using DetailAndGo.Models;
using DetailAndGo.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DetailAndGo.Services
{
    public class CarService : ICarService
    {
        public ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CarService(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {

            _context = context;
            _userManager = userManager;

        }

        public async Task SaveCar(Car carToSave)
        {
            _context.Cars.Add(carToSave);
            await _context.SaveChangesAsync();            
        }

        public async Task<List<Car>> GetCustomerCars(string aspNetUserId)
        {
            List<Car> customerCars = await _context.Cars.Where(s => s.AspNetUserId == aspNetUserId).ToListAsync();
            return customerCars;
        }

        public async Task<Car> GetCustomerActiveCar(string aspNetUserId)
        {
            Car car = await _context.Cars.Where(s => s.AspNetUserId == aspNetUserId && s.IsPrimary == true).FirstOrDefaultAsync();
            return car;
        }

        public async Task RemoveCar(Car car)
        {
            
            _context.Cars.Remove(car);
            _context.SaveChanges();
        }

        public async Task MakeCarActive(int carId, string aspNetUserId)
        {
            List<Car> customerCars = await GetCustomerCars(aspNetUserId);
            foreach(Car car in customerCars)
            {
                car.IsPrimary = false;
            }
            customerCars.Where(s => s.Id == carId).FirstOrDefault().IsPrimary = true;
            _context.UpdateRange(customerCars);
            _context.SaveChanges(); 

            var test = _context.Cars.FirstOrDefault(s => s.IsPrimary == true);
        }

        public async Task<Car> GetCarByIndex(string aspNetUserId, int index)
        {
            List<Car> allCars = await GetCustomerCars(aspNetUserId);
            return allCars[index];
        }

        public async Task<List<CarHistory>> GetCarHistoryByCarId(int carId)
        {
            List<CarHistory> carHistories = await _context.CarHistories.Where(s => s.CarId == carId).ToListAsync();
            return carHistories;
        }

        public async Task AddToCarHistory(CarHistory carHistory)
        {
            _context.CarHistories.Add(carHistory);
            await _context.SaveChangesAsync();
        }

        public string GetCarName(int carId)
        {
            Car car = _context.Cars.FirstOrDefault(s => s.Id == carId);

            return car.CarModel + " " + car.CarFamily;

            
        }
    }
}
