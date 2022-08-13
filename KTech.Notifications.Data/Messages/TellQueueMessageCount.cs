namespace KTech.Notifications.Data.Messages
{
    public class TellQueueMessageCount
    {
        public string QueueName { get; private set; }
        public uint MessageCount { get; private set; }

        public TellQueueMessageCount(string queueName, uint messageCount)
        {
            QueueName = queueName;
            MessageCount = messageCount;
        }
    }
}
