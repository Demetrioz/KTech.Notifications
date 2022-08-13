namespace KTech.Notifications.Data.Messages
{
    public class AskForQueueMessageCount
    {
        public string QueueName { get; private set; }

        public AskForQueueMessageCount(string queueName)
        {
            QueueName = queueName;
        }
    }
}
