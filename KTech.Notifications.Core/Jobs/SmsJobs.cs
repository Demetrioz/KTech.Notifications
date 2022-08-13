using KTech.Notifications.Core.Services.SmsService;
using KTech.Notifications.Data.DTOs;

namespace KTech.Notifications.Core.Jobs
{
    public class SmsJobs
    {
        private readonly ISmsService _smsService;

        public SmsJobs(ISmsService smsService)
        {
            _smsService = smsService;
        }

        public void ScheduleSMS(ScheduledSmsData sms)
        {
            _smsService.SendSmsAsync(sms);
        }
    }
}
