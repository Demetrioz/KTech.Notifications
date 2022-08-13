using KTech.Notifications.Data.DTOs;
using System.Net;

namespace KTech.Notifications.Services.EmailService
{
    public interface IEmailService
    {
        Task<HttpStatusCode?> SendEmailAsync(EmailData email);
    }
}
