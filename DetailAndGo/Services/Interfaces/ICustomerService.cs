using DetailAndGo.Models;

namespace DetailAndGo.Services.Interfaces
{
    public interface ICustomerService
    {
        public Customer GetCustomerById(string? customerId);
        public Task<bool> RegisterCustomerAsync(Customer customer);
    }
}
