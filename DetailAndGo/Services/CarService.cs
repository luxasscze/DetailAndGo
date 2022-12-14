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
    }
}
