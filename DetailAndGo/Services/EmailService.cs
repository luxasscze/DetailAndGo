using DetailAndGo.Models;
using DetailAndGo.Services.Interfaces;
using DetailAndGo.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;

namespace DetailAndGo.Services
{

    public class EmailService : IEmailService
    {        
        private IWebHostEnvironment _webHostEnvironment;
        private ICustomerService _customerService;
        private IDAGService _serviceService;
        private GeneralUtility _generalUtility;
        private string _host = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("BrevoEmail")["Host"];
        private string _userName = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("BrevoEmail")["Username"];
        private string _password = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("BrevoEmail")["Password"];
        private string _ssl = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("BrevoEmail")["SSL"];
        private string _port = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("BrevoEmail")["Port"];

        public EmailService(IWebHostEnvironment webHostEnvironment, ICustomerService customerService, IDAGService serviceService)
        {
            _webHostEnvironment = webHostEnvironment;
            _customerService = customerService;
            _generalUtility = new GeneralUtility();
            _serviceService = serviceService;
        }

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

        public async Task<bool> SendBookingCreatedEmail(string emailTo, Booking booking)
        {
            string firstName = _customerService.GetCustomerByEmail(emailTo).FirstName;
            string emailBody = File.ReadAllText(_webHostEnvironment.WebRootPath + "/Email/booking/created.html");
            List<Service> allMainServices = await _serviceService.GetServicesFromIdArray(booking.ServicesArray);

            Email email = new Email()
            {
                From = _userName,
                Body = emailBody.Replace("{firstName}", firstName).Replace("{bookingId}", booking.Id.ToString()).
                Replace("{amountPaid}", _generalUtility.ConvertPriceToDecimal(booking.TotalAmount).ToString()).
                Replace("{bookedFor}", booking.BookedFor.ToString("dd/MM/yyyy HH:mm")).
                Replace("{services}", _generalUtility.GetServiceNamesForEmail(allMainServices, booking)),
                IsHtml = true,
                Subject = "DETAIL&GO Booking has been created",
                To = emailTo
            };
            return await SendSingleEmail(email);
        }

        public async Task<bool> SendBookingDeclinedEmail(string emailTo, string reason)
        {
            string firstName = _customerService.GetCustomerByEmail(emailTo).FirstName;
            string emailBody = File.ReadAllText(_webHostEnvironment.WebRootPath + "/Email/booking/declined.html");
            Email email = new Email()
            {
                Body = emailBody.Replace("{firstName}", firstName).Replace("{reason}", reason),
                From = _userName,
                Subject = "DETAIL&GO booking has been declined!",
                IsHtml = true,
                To = emailTo
            };
            return await SendSingleEmail(email);
        }

        public async Task<bool> SendBookingApprovedEmail(string emailTo, Booking booking)
        {
            string firstName = _customerService.GetCustomerByEmail(emailTo).FirstName;
            string emailBody = File.ReadAllText(_webHostEnvironment.WebRootPath + "/Email/booking/approved.html");

            Email email = new Email()
            {
                Body = emailBody.Replace("{bookingId}", booking.Id.ToString()).Replace("{firstName}", firstName).
                Replace("{amountPaid}", _generalUtility.ConvertPriceToDecimal(booking.TotalAmount).ToString()).
                Replace("{bookedFor}", booking.BookedFor.ToString("dd/MM/yyyy HH:mm")).
                Replace("{services}", booking.ServicesArray),
                From = _userName,
                IsHtml = true,
                Subject = "DETAIL&GO booking has been approved!",
                To = emailTo
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
