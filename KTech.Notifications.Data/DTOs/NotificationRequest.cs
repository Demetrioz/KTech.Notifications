namespace KTech.Notifications.Data.DTOs
{
    public class NotificationRequest
    {
        public Type NotificationType { get; set; }
        public object NotificationData { get; set; }
    }
}
