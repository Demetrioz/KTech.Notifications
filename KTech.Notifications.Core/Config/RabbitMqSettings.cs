namespace KTech.Notifications.Core.Config
{
    public class RabbitMqSettings
    {
        // Connection Information
        public string HostName { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        // Queue Checking
        public string NotificationQueue { get; set; }
        public int QueueCheckIntervalMinutes { get; set; }

        // Subscriber Settings
        public int MinimumSubscribers { get; set; }
        public int WorkersPerSubscriber { get; set; }

        // Subscriber Scaling
        public int MessageCountDivisor { get; set; }
        public int SubscribersPerMessageCount { get; set; }
    }
}
