using System.Net;
using System.Net.Mail;
using DetailAndGo.Models;

namespace DetailAndGo.Services.Interfaces
{
    public interface IEmailService
    {
        public Task<bool> SendSingleEmail(Email email);
        public Task<bool> SendBookingDeclinedEmail(string mailTo, string reason);
        public Task<bool> SendBookingCancelledEmail(string emailTo);
        public Task<bool> SendBookingConfirmedEmail(string emailTo, Booking booking);
        public Task<bool> SendBookingUpdateEmail(string emailTo, Booking booking);
    }
}
