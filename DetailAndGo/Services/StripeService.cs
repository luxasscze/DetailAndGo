using DetailAndGo.Data;
using DetailAndGo.Models;
using DetailAndGo.Services.Interfaces;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stripe;
using Microsoft.Extensions.Configuration;
using Stripe.TestHelpers;

namespace DetailAndGo.Services
{
    public class StripeService : IStripeService // TRY TO MOVE ALL STRIPE SERVICES AS THEY SHOULD BE LOCATED
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

        public async Task AttachPaymentMethodToCustomer(string customerId, string paymentMethodId)
        {
            StripeConfiguration.ApiKey = _stripeApiKey;

            var options = new PaymentMethodAttachOptions
            {
                Customer = customerId,
            };
            var service = new PaymentMethodService();
            await service.AttachAsync(paymentMethodId, options);
        }

        public async Task<StripeList<PaymentMethod>> GetCustomerPaymentMethods(string stripeId)
        {
            StripeConfiguration.ApiKey = _stripeApiKey;

            PaymentMethodService paymentMethodService = new PaymentMethodService();
            PaymentMethodListOptions listOptions = new PaymentMethodListOptions()
            {
                Customer = stripeId,
                Type = "card",
            };
            StripeList<PaymentMethod> result = await paymentMethodService.ListAsync(listOptions);
            return result;            
        }

        public async Task<Charge> ChargeCustomerForBooking(Models.Customer customer, long amount)
        {
            StripeConfiguration.ApiKey = _stripeApiKey;

            ChargeCreateOptions options = new ChargeCreateOptions()
            {
                Amount = amount,
                Currency = "gbp",
                Customer = customer.StripeId,
                ReceiptEmail = customer.Email,                
            };

            ChargeService chargeService = new ChargeService();
            Charge result = await chargeService.CreateAsync(options);
            return result;
        }

        public async Task<Product> CreateProduct(string productName, string description, List<decimal> price, Dictionary<string, string> metadata)
        {
            StripeConfiguration.ApiKey = _stripeApiKey;
            ProductService service = new ProductService();
            PriceService priceService = new PriceService();
            ProductCreateOptions options = new ProductCreateOptions();

            if (price.Count > 1)
            {
                options = new ProductCreateOptions()
                {
                    Active = true,                    
                    Description = description,
                    Name = productName,
                    StatementDescriptor = "DETAIL&GO",
                    Metadata = metadata
                };

                Product product = await service.CreateAsync(options);

                int count = 0;
                foreach (var singlePrice in price)
                {
                    var priceOptions = new PriceCreateOptions()
                    {
                        UnitAmount = (long)singlePrice * 100,
                        Currency = "gbp",
                        Product = product.Id,                        
                    };

                    if(count == 0)
                    {
                        priceOptions.Nickname = "small";
                    }
                    else if (count == 1)
                    {
                        priceOptions.Nickname = "medium";
                    }
                    else
                    {
                        priceOptions.Nickname = "large";
                    }

                    await priceService.CreateAsync(priceOptions);
                    count++;
                }                

                return product;
            }
            else
            {

                options = new ProductCreateOptions()
                {
                    Active = true,
                    DefaultPriceData = new ProductDefaultPriceDataOptions()
                    {
                        Currency = "gbp",
                        UnitAmountDecimal = price[0] * 100,
                    },
                    Description = description,
                    Name = productName,
                    StatementDescriptor = "DETAIL&GO",
                    Metadata = metadata
                };
                return await service.CreateAsync(options);
            }
            
        }

        // TODO: create model as one parameter
        public async Task<Product> UpdateProduct(string productId, string productName, string description, decimal price, decimal priceMedium, decimal priceLarge, Dictionary<string, string> metadata)
        {
            StripeConfiguration.ApiKey = _stripeApiKey;

            PriceCreateOptions priceOptions = new PriceCreateOptions()
            {
                Active = true,
                Currency = "gbp",
                Product = productId,
                UnitAmountDecimal = price * 100,
                Nickname = "small"
            };

            PriceCreateOptions priceMediumOptions = new PriceCreateOptions()
            {
                Active = true,
                Currency = "gbp",
                Product = productId,
                UnitAmountDecimal = priceMedium * 100,
                Nickname = "medium"
            };

            PriceCreateOptions priceLargeOptions = new PriceCreateOptions()
            {
                Active = true,
                Currency = "gbp",
                Product = productId,
                UnitAmountDecimal = priceLarge * 100,
                Nickname = "large"
            };

            PriceService priceService = new PriceService();
            Price newPrice = await priceService.CreateAsync(priceOptions);
            Price newPriceMedium = await priceService.CreateAsync(priceMediumOptions);
            Price newPriceLarge = await priceService.CreateAsync(priceLargeOptions);

            ProductUpdateOptions options = new ProductUpdateOptions()
            {
                Name = productName,
                DefaultPrice = newPrice.Id,
                Description = description,
                Metadata = metadata
            };

            ProductService service = new ProductService();
            Product updatedProduct = await service.UpdateAsync(productId, options);
            return updatedProduct;
        }

        public async Task<string> MakePaymentMethodDefault()
        {
           CardService service = new CardService();
            CardUpdateOptions opt = new CardUpdateOptions()
            {
                
            };
            
            return "";
        }

        public async Task<Stripe.Customer> SetCustomerDefaultPaymentMethod(string customerId, string paymentMethodId)
        {
            CustomerUpdateOptions options = new CustomerUpdateOptions()
            {
                //DefaultSource = paymentMethodId,
                InvoiceSettings = new CustomerInvoiceSettingsOptions()
                {
                    DefaultPaymentMethod = paymentMethodId
                }
            };
            
            Stripe.CustomerService service = new Stripe.CustomerService();
            return await service.UpdateAsync(customerId, options);
        }

        public string GetCustomerDefaultPaymentMethod(string stripeId)
        {
            string result = string.Empty;
            StripeConfiguration.ApiKey = _stripeApiKey;           
            Stripe.CustomerService service = new Stripe.CustomerService();
            result = service.Get(stripeId).InvoiceSettings.DefaultPaymentMethodId;
            return string.IsNullOrEmpty(result) ? service.Get(stripeId).DefaultSourceId : result;
        }

        public string GetLast4(string stripeId)
        {
            StripeConfiguration.ApiKey = _stripeApiKey;
            Stripe.CustomerService service = new Stripe.CustomerService();
            var test = service.Get(stripeId);
            string defaultId = service.Get(stripeId).InvoiceSettings.DefaultPaymentMethodId;
            if(string.IsNullOrEmpty(defaultId))
            {
                defaultId = service.Get(stripeId).DefaultSourceId;
            }
            if(string.IsNullOrEmpty(defaultId))
            {
                return "N/A"; // NO DEFAULT PAYMENT FOUND
            }
            PaymentMethodService ser = new PaymentMethodService();
            string result = ser.Get(defaultId).Card.Last4;
            return result;
        }

        public async Task<Stripe.Card> RemovePaymentMethod(string stripeId, string paymentMethodId)
        {
            StripeConfiguration.ApiKey = _stripeApiKey;
            CardService service = new CardService();            
            if(paymentMethodId.Contains("pm_"))
            {
                PaymentMethodService pmService = new PaymentMethodService();
                await pmService.DetachAsync(paymentMethodId);
            }
            return await service.DeleteAsync(stripeId, paymentMethodId);            
        }

        public async Task<PaymentIntent> CreatePaymentIntent(string customer, string paymentMethodId, long amount)
        {
            PaymentIntentCreateOptions options = new PaymentIntentCreateOptions()
            {
                Amount = amount,
                Currency = "gbp",
                Customer = customer,
                Description = "paymnet intend test",
                PaymentMethod = paymentMethodId,
                PaymentMethodTypes = new List<string>()
                {
                    "card"
                },
                ReceiptEmail = "lukas2slivka@gmail.com"                
            };

            PaymentIntentService service = new PaymentIntentService();
            return await service.CreateAsync(options);
        }

        public async Task<StripeList<Price>> GetPricesByProductId(string productId)
        {
            StripeConfiguration.ApiKey = _stripeApiKey;
            PriceService service = new PriceService();
            PriceListOptions options = new PriceListOptions()
            {
                Product = productId
            };

            return await service.ListAsync(options);
        }

        public async Task DeactivatePrice(string priceId)
        {
            StripeConfiguration.ApiKey = _stripeApiKey;
            PriceService service = new PriceService();
            PriceUpdateOptions options = new PriceUpdateOptions()
            {
                Active = false
            };
            await service.UpdateAsync(priceId, options);
        }

        public async Task<Price> CreatePrice(long? amount, string productId, string nickname)
        {
            StripeConfiguration.ApiKey = _stripeApiKey;
            PriceService service = new PriceService();
            PriceCreateOptions options = new PriceCreateOptions()
            {
                Active = true,
                Currency = "gbp",
                Product = productId,
                UnitAmount = amount,
                Nickname = nickname
            };
            return await service.CreateAsync(options);
        }

        public async Task<Product> GetProductById(string productId)
        {
            StripeConfiguration.ApiKey = _stripeApiKey;
            ProductService service = new ProductService();
            Product product = await service.GetAsync(productId);
            return product;
        }

    }
}
