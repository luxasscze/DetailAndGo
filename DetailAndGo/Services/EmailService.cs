using DetailAndGo.Services.Interfaces;
using System.Net.Mail;
using System.Net;

namespace DetailAndGo.Services
{
    
    public class EmailService : IEmailService
    {
        private string _host = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Email")["Host"];
        private string _userName = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Email")["Username"];
        private string _password = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Email")["Password"];
        public async Task SendSingleEmail(Models.Email email)
        {
            
            using (var smtp = new SmtpClient())
            {
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                //smtp.PickupDirectoryLocation = @"c:\maildump";
                smtp.Host = _host;
                //smtp.Port = 465;
                //smtp.EnableSsl = true;
                
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
                }
                catch(Exception ex)
                {
                    var result = ex;
                }
            }
        }
    }
}
