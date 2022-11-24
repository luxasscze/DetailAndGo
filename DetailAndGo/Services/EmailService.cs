using DetailAndGo.Services.Interfaces;
using System.Net.Mail;
using System.Net;

namespace DetailAndGo.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendSingleEmail(Models.Email email)
        {
            using (var smtp = new SmtpClient())
            {
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                //smtp.PickupDirectoryLocation = @"c:\maildump";
                smtp.Host = "smtp.livemail.co.uk";
                smtp.Credentials = new NetworkCredential()
                {
                    UserName = "info@detailandgo.co.uk",
                    Password = "Gordon2Freeman1*"
                };
                var message = new MailMessage
                {
                    Body = email.Body,
                    Subject = email.Subject,
                    From = new MailAddress(email.From)
                };
                message.To.Add(email.To);
                await smtp.SendMailAsync(message);
            }
        }
    }
}
