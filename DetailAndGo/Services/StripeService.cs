using DetailAndGo.Data;
using DetailAndGo.Models;
using DetailAndGo.Services.Interfaces;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stripe;
using Microsoft.Extensions.Configuration;

namespace DetailAndGo.Services
{
    public class StripeService : IStripeService
    {
        public ApplicationDbContext _context;
        private readonly string _stripeApiKey;

        public StripeService(ApplicationDbContext context)
        {
            _context = context;
            _stripeApiKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Stripe")["ApiKey"];
        }

        public Task<string> CreateCustomerAsync(Models.Customer customer)
        {
            StripeConfiguration.ApiKey = _stripeApiKey;
            AddressOptions addressOptions = new AddressOptions()
            {
                PostalCode = customer.PostCode,
                Line1 = customer.Address1,
                Line2 = customer.Address2,
                City = customer.Address3                
            };

            var options = new CustomerCreateOptions
            {
                Description = customer.FirstName + ", " + customer.LastName,
                Email = customer.Email,
                Address = addressOptions,
                Name = customer.FirstName + " " + customer.LastName,                
            };
            var service = new Stripe.CustomerService();
            Stripe.Customer newCustomer = service.Create(options);

            if(newCustomer != null)
            {
                return Task.FromResult(newCustomer.Id);
            }

            return Task.FromResult("");
        }
    }
}
