namespace KTech.Notifications.Data.DTOs
{
    public class ScheduledEmailData : EmailData
    {
        public DateTimeOffset UtcScheduledTime { get; set; }
    }
}
