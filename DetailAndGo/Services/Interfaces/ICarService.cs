using DetailAndGo.Models;

namespace DetailAndGo.Services.Interfaces
{
    public interface ICarService
    {
        public Task SaveCar(Car carToSave);
        public Task<List<Car>> GetCustomerCars(string aspNetUserId);
        public Task<Car> GetCustomerActiveCar(string aspNetUserId);
    }
}
