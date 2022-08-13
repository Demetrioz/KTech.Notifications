using KTech.Notifications.Core.Services.EmailService;
using KTech.Notifications.Data.DTOs;

namespace KTech.Notifications.Core.Jobs
{
    public class EmailJobs
    {
        private readonly IEmailService _emailService;

        public EmailJobs(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public void ScheduleEmail(ScheduledEmailData email)
        {
            _emailService.SendEmailAsync(email);
        }
    }
}
