using System.Net;
using System.Net.Mail;

namespace DetailAndGo.Services.Interfaces
{
    public interface IEmailService
    {
        public Task SendSingleEmail(Models.Email email);
    }
}
