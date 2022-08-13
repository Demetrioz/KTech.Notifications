using KTech.Notifications.Data.DTOs;
using System.Net;

namespace KTech.Notifications.Core.Services.EmailService
{
    public interface IEmailService
    {
        Task<HttpStatusCode?> SendEmailAsync(EmailData email);
    }
}
