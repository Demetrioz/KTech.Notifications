using Akka.Actor;
using Hangfire;
using KTech.Notifications.Core.Jobs;
using KTech.Notifications.Core.Services.EmailService;
using KTech.Notifications.Core.Services.SmsService;
using KTech.Notifications.Data.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace KTech.Notifications.Core.Actors
{
    public class NotificationWorker : ReceiveActor
    {
        private readonly IServiceScope _scope;
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;

        public NotificationWorker(IServiceProvider provider)
        {
            _scope = provider.CreateScope();

            _emailService = _scope.ServiceProvider
                .GetRequiredService<IEmailService>();
            _smsService = _scope.ServiceProvider
                .GetRequiredService<ISmsService>();

            Receive<NotificationRequest>(HandleNotification);
        }

        protected override void PostStop()
        {
            _scope.Dispose();
        }

        private void HandleNotification(NotificationRequest message)
        {
            try
            {
                string json = JsonConvert.SerializeObject(message.NotificationData);
                var typedNotification = JsonConvert.DeserializeObject(json, message.NotificationType);

                // TODO: Sender.Tell(uid) instead of true/false
                switch (typedNotification)
                {
                    case ScheduledEmailData sed:
                        BackgroundJob.Schedule<EmailJobs>(x =>
                            x.ScheduleEmail(sed),
                            sed.UtcScheduledTime);
                        break;
                    case EmailData ed:
                        _emailService.SendEmailAsync(ed).Wait();
                        break;
                    case ScheduledSmsData ssd:
                        BackgroundJob.Schedule<SmsJobs>(x =>
                            x.ScheduleSMS(ssd),
                            ssd.UtcScheduledTime);
                        break;
                    case SmsData sd:
                        _smsService.SendSmsAsync(sd).Wait();
                        break;

                    default:
                        break;
                }

                Sender.Tell(true);
            }
            catch (Exception ex)
            {
                // TODO: Log Error
                Sender.Tell(false);
            }
        }
    }
}
