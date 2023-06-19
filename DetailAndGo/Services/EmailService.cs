using DetailAndGo.Services.Interfaces;
using System.Net.Mail;
using System.Net;
using DetailAndGo.Models;

namespace DetailAndGo.Services
{

    public class EmailService : IEmailService
    {
        private string _host = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("BrevoEmail")["Host"];
        private string _userName = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("BrevoEmail")["Username"];
        private string _password = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("BrevoEmail")["Password"];
        private string _ssl = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("BrevoEmail")["SSL"];
        private string _port = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("BrevoEmail")["Port"];
        public async Task<bool> SendSingleEmail(Email email)
        {
            if (string.IsNullOrEmpty(email.From))
            {
                email.From = _userName;
            }

            using (var smtp = new SmtpClient())
            {
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Host = _host;
                smtp.Port = int.Parse(_port);
                smtp.EnableSsl = bool.Parse(_ssl);


                smtp.Credentials = new NetworkCredential()
                {
                    UserName = _userName,
                    Password = _password
                };
                var message = new MailMessage
                {
                    Body = email.Body,
                    Subject = email.Subject,
                    From = new MailAddress(email.From),
                    IsBodyHtml = email.IsHtml
                };
                message.To.Add(email.To);
                try
                {
                    await smtp.SendMailAsync(message);
                    return true;
                }
                catch (Exception ex)
                {
                    var result = ex;
                    return false;
                }
            }
        }

        public async Task<bool> SendBookingDeclinedEmail(string mailTo, string reason)
        {
            Email email = new Email()
            {
                Body = "Your booking has been declined. <br/> Please, Read the reason below: <br/> Reason: " + reason,
                From = _userName,
                Subject = "DETAIL&GO booking has been declined!",
                IsHtml = true,
                To = mailTo
            };
            return await SendSingleEmail(email);
        }

        public async Task<bool> SendBookingCancelledEmail(string emailTo)
        {
            Email email = new Email()
            {
                Body = "Your booking has been cancelled by you",
                From = _userName,
                IsHtml = true,
                Subject = "Your DETAIL&GO booking has been cancelled",
                To = emailTo
            };
            return await SendSingleEmail(email);
        }

        public async Task<bool> SendBookingConfirmedEmail(string emailTo, Booking booking)
        {
            Email email = new Email()
            {
                Body = "Your booking #" + booking.Id + " has been confirmed.",
                From = _userName,
                IsHtml = true,
                Subject = "DETAIL&GO Booking #" + booking.Id,
                To = emailTo
            };
            return await SendSingleEmail(email);
        }

        public async Task<bool> SendBookingUpdateEmail(string emailTo, Booking booking)
        {
            Email email = new Email()
            {
                Body = "Your booking has been updated to " + booking.Status,
                From = _userName,
                IsHtml = true,
                Subject = "Your DETAIL&GO booking has been updated",
                To = emailTo
            };
            return await SendSingleEmail(email);
        }
    }
}
