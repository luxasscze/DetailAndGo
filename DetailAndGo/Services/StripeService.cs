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
            _stripeApiKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Stripe")["ApiKeyTest"];
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

        public Task<string> CreatePaymentMethod(string cardNumber, int expMonth, int expYear, string cvc)
        {
            StripeConfiguration.ApiKey = _stripeApiKey;

            var options = new PaymentMethodCreateOptions
            {
                Type = "card",
                Card = new PaymentMethodCardOptions
                {
                    Number = cardNumber,
                    ExpMonth = expMonth,
                    ExpYear = expYear,
                    Cvc = cvc                    
                },
            };
            var service = new PaymentMethodService();
            PaymentMethod paymentMethod = new PaymentMethod();
            try
            {
                paymentMethod = service.Create(options);
            }
            catch (Exception ex)
            {
                return Task.FromResult("invalid_card");
            }
            if (paymentMethod != null)
            {
                return Task.FromResult(paymentMethod.Id);
            }
            return Task.FromResult("");
        }        

        public void AttachPaymentMethodToCustomer(string customerId, string paymentMethodId)
        {
            StripeConfiguration.ApiKey = _stripeApiKey;

            var options = new PaymentMethodAttachOptions
            {
                Customer = customerId,
            };
            var service = new PaymentMethodService();
            service.Attach(
              paymentMethodId,
              options);
        }

        public async Task<StripeList<PaymentMethod>> GetCustomerPaymentMethods(string stripeId)
        {
            StripeConfiguration.ApiKey = _stripeApiKey;

            PaymentMethodService paymentMethodService = new PaymentMethodService();
            PaymentMethodListOptions listOptions = new PaymentMethodListOptions()
            {
                Customer = stripeId,
                Type = "card"
            };
            StripeList<PaymentMethod> result = await paymentMethodService.ListAsync(listOptions);
            return result;            
        }

        public async Task<Charge> ChargeCustomerForBooking(Models.Customer customer, long amount)
        {
            StripeConfiguration.ApiKey = _stripeApiKey;

            ChargeCreateOptions options = new ChargeCreateOptions()
            {
                Amount = amount * 100,
                Currency = "gbp",
                Customer = customer.StripeId,
                ReceiptEmail = customer.Email,                
            };

            ChargeService chargeService = new ChargeService();
            Charge result = await chargeService.CreateAsync(options);
            return result;
        }

        public async Task<Product> CreateProduct(string productName, string description, decimal price)
        {
            StripeConfiguration.ApiKey = _stripeApiKey;

            ProductCreateOptions options = new ProductCreateOptions()
            {
                Active = true,                
                DefaultPriceData = new ProductDefaultPriceDataOptions()
                {
                    Currency = "gbp",
                    UnitAmountDecimal = price * 100,                    
                },
                Description = description,
                Name = productName,
                StatementDescriptor = "DETAIL&GO",                
            };
            ProductService service = new ProductService();
            return await service.CreateAsync(options);
        }

        public async Task<Product> UpdateProduct(string productId, string productName, string description, decimal price)
        {
            StripeConfiguration.ApiKey = _stripeApiKey;

            PriceCreateOptions priceOptions = new PriceCreateOptions()
            {
                Active = true,
                Currency = "gbp",
                Product = productId,
                UnitAmountDecimal = price * 100,
            };            

            PriceService priceService = new PriceService();
            Price newPrice = await priceService.CreateAsync(priceOptions);            

            ProductUpdateOptions options = new ProductUpdateOptions()
            {
                Name = productName,
                DefaultPrice = newPrice.Id,
                Description = description
            };

            ProductService service = new ProductService();
            Product updatedProduct = await service.UpdateAsync(productId, options);
            return updatedProduct;
        }
    }
}
