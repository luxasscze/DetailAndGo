using Stripe;

namespace DetailAndGo.Services.Interfaces
{
    public interface IStripeService
    {
        public Task<string> CreateCustomerAsync(Models.Customer customer);
        public Task<string> CreatePaymentMethod(string cardNumber, int expMonth, int expYear, string cvc);
        public Task AttachPaymentMethodToCustomer(string customerId, string paymentMethodId);
        public Task<StripeList<PaymentMethod>> GetCustomerPaymentMethods(string stripeId);
        public Task<PaymentIntent> ChargeCustomerForBooking(Models.Customer customer, Models.Booking booking);
        public Task<Product> CreateProduct(string productName, string description, List<decimal> price, Dictionary<string, string> metadata);
        public Task<Product> UpdateProduct(string productId, string productName, string description, decimal price, decimal priceMedium, decimal priceLarge, Dictionary<string, string> metadata);
        public Task<Stripe.Customer> SetCustomerDefaultPaymentMethod(string customerId, string paymentMethodId);
        public Task<PaymentIntent> CreatePaymentIntent(string customer, string paymentMethodId, long amount);
        public string GetCustomerDefaultPaymentMethod(string stripeId);
        public string GetLast4(string stripeId);
        public Task<Stripe.Card> RemovePaymentMethod(string stripeId, string paymentMethodId);
        public Task<StripeList<Price>> GetPricesByProductId(string productId);
        public Task DeactivatePrice(string priceId);
        public Task<Price> CreatePrice(long? amount, string productId, string nickname);
        public Task<Refund> Refund(string paymentIntentId, string reason, long amount);
        public Task<Product> GetProductById(string productId);
    }
}
