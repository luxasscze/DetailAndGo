namespace DetailAndGo.Services.Interfaces
{
    public interface IStripeService
    {
        public Task<string> CreateCustomerAsync(Models.Customer customer);
    }
}
