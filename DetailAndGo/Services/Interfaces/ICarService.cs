using DetailAndGo.Models;

namespace DetailAndGo.Services.Interfaces
{
    public interface ICarService
    {
        public Task SaveCar(Car carToSave);
        public Task<List<Car>> GetCustomerCars(string aspNetUserId);
        public Task<Car> GetCustomerActiveCar(string aspNetUserId);
        public Task RemoveCar(Car car);
        public Task MakeCarActive(int carId, string aspNetUserId);
        public Task<Car> GetCarByIndex(string aspNetUserId, int index);
        public Task<List<CarHistory>> GetCarHistoryByCarId(int carId);
        public Task AddToCarHistory(CarHistory carHistory);
        public string GetCarName(int carId);
        public Task<Car?> GetCarById(int id);
    }
}
