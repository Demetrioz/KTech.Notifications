namespace KTech.Notifications.Data.DTOs
{
    public class ScheduledSmsData : SmsData
    {
        public DateTimeOffset UtcScheduledTime { get; set; }
    }
}
