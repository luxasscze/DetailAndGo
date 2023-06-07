using DetailAndGo.Models;
using DetailAndGo.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stripe;

namespace DetailAndGoAdmin.Pages.Jobs
{
    [Authorize]
    public class JobDetailModel : PageModel
    {
        private readonly IBookingService _bookingService;
        private readonly ICarService _carService;
        private readonly IDAGService _serviceService;
        private readonly IEmailService _emailService;
        private readonly IStripeService _stripeService;
        private readonly ICustomerService _customerService;

        public JobDetailModel(IBookingService bookingService, ICarService carService, IDAGService serviceService, IEmailService emailService, IStripeService stripeService, ICustomerService customerService)
        {
            _bookingService = bookingService;
            _carService = carService;
            _serviceService = serviceService;
            _emailService = emailService;
            _stripeService = stripeService;
            _customerService = customerService;
        }

        public Booking booking { get; set; }
        public Car? car { get; set; }
        public List<Service> services {get; set;}
        public List<Service> subServices { get; set; }
        public List<BookingHistory> History { get; set; }

        public async Task OnGet(int id)
        {
            booking = await _bookingService.GetBookingById(id);
            car = await _carService.GetCarById(booking.CarId);
            services = await GetServicesFromArray(booking.ServicesArray);
            subServices = await _serviceService.GetServicesFromIdArray(booking.SubServicesArray);
            History = await _bookingService.GetBookingHistoryByBookingId(booking.Id);
        }

        public async Task<List<Service>> GetServicesFromArray(string array)
        {
           return await _serviceService.GetServicesFromIdArray(array);
        }

        public async Task<ActionResult> OnGetDeclineBooking(int bookingId,string reason)
        {
            await _bookingService.DeclineBooking(bookingId, reason);
            Email email = new Email()
            {
                Body = "You sucks, your booking has been declined!<br /> Reason: <br /> " + reason,
                From = "admin@detailandgo.co.uk",
                IsHtml = true,
                Subject = "Detail&Go booking has been declined!",
                To = "lukas2slivka@gmail.com"
            };
            await _emailService.SendSingleEmail(email);
            return RedirectToPage("JobDetail", new { id = bookingId});
        }

        public async Task<ActionResult> OnGetReinstateBooking(int bookingId)
        {
            await _bookingService.ReinstateBooking(bookingId);
            return RedirectToPage("JobDetail", new { id = bookingId });
        }

        public async Task<ActionResult> OnGetOnTheWayBooking(int bookingId)
        {
            await _bookingService.SetBookingOnTheWay(bookingId);
            return RedirectToPage("JobDetail", new { id = bookingId });
        }

        public async Task<ActionResult> OnGetStartJob(int bookingId)
        {
            await _bookingService.StartJob(bookingId);
            return RedirectToPage("JobDetail", new { id = bookingId });
        }

        public async Task<ActionResult> OnGetFinishBooking(int bookingId)
        {
            await _bookingService.FinishBooking(bookingId);
            return RedirectToPage("JobDetail", new { id = bookingId });
        }

        public async Task<ActionResult> OnGetAcceptBooking(int bookingId)
        {
            Booking booking = await _bookingService.GetBookingById(bookingId);
            DetailAndGo.Models.Customer customer = _customerService.GetCustomerById(booking.AspNetUserId);
            try
            {
                PaymentIntent paymentIntent = await _stripeService.ChargeCustomerForBooking(customer, booking);
                if (paymentIntent.Status == "succeeded")
                {
                    await _bookingService.AcceptBooking(bookingId);
                }
                else if (paymentIntent.Status == "required_action")
                {

                }
            }
            catch(StripeException ex)
            {
                var test = ex.StripeError.Code;

                if(ex.StripeError.Code == "card_declined")
                {
                    BookingHistory history = new BookingHistory()
                    {
                        BookingId = bookingId,
                        Created = DateTime.Now,
                        Description = "Payment method " + booking.PaymentMethodId + " has been declined",
                        Status = DetailAndGo.Models.Enums.BookingStatus.CardDeclined
                    };
                    await _bookingService.AddToBookingHistory(history);
                    await _bookingService.UpdateBookingStatus(bookingId, DetailAndGo.Models.Enums.BookingStatus.CardDeclined, "Your payment card has been declined. We will try another attempt soon, or you can update your card below");
                    return RedirectToPage("JobDetail", new { id = bookingId, message = "card_declined" });
                }
                
            }

                   
            return RedirectToPage("JobDetail", new { id = bookingId });
        }
    }
}
