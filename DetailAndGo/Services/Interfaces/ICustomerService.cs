using DetailAndGo.Models;

namespace DetailAndGo.Services.Interfaces
{
    public interface ICustomerService
    {
        public Customer GetCustomerById(string? customerId);
        public Customer GetCustomerByEmail(string? email);
        public Task<bool> RegisterCustomerAsync(Customer customer);
        public bool CheckEmailExists(string email);
    }
}
