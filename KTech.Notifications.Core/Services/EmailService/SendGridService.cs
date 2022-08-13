using KTech.Notifications.Data.DTOs;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;

namespace KTech.Notifications.Core.Services.EmailService
{
    public class SendGridService : IEmailService
    {
        private readonly ISendGridClient _client;

        public SendGridService(ISendGridClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(ISendGridClient));
        }

        public async Task<HttpStatusCode?> SendEmailAsync(EmailData email)
        {
            // TODO: Handle errors
            // TODO: Handle failures / retries

            SendGridMessage message = new()
            {
                From = new EmailAddress(email.From),
                Subject = email.Subject
            };

            message.AddContent(MimeType.Html, email.Body);

            foreach (string to in email.To)
                message.AddTo(new EmailAddress(to));

            if (email.Bcc != null && email.Bcc.Length > 0)
                foreach (string bcc in email.Bcc)
                    message.AddBcc(new EmailAddress(bcc));

            Response? response = await _client.SendEmailAsync(message);
            return response?.StatusCode;
        }
    }
}
