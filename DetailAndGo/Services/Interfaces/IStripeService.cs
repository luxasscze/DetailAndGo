using Stripe;

namespace DetailAndGo.Services.Interfaces
{
    public interface IStripeService
    {
        public Task<string> CreateCustomerAsync(Models.Customer customer);
        public Task<string> CreatePaymentMethod(string cardNumber, int expMonth, int expYear, string cvc);
        public void AttachPaymentMethodToCustomer(string customerId, string paymentMethodId);
        public Task<StripeList<PaymentMethod>> GetCustomerPaymentMethods(string aspNetUserId);
    }
}
