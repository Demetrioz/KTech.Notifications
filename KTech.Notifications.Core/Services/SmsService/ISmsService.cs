using KTech.Notifications.Data.DTOs;

namespace KTech.Notifications.Core.Services.SmsService
{
    public interface ISmsService
    {
        Task<string?> SendSmsAsync(SmsData sms);
    }
}
