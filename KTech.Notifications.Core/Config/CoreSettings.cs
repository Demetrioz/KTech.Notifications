namespace KTech.Notifications.Core.Config
{
    public class CoreSettings
    {
        public SendGridSettings SendGrid { get; set; }
        public TwilioSettings Twilio { get; set; }
        public RabbitMqSettings RabbitMq { get; set; }
    }
}
