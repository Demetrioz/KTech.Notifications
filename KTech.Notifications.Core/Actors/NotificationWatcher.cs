using Akka.Actor;
using KTech.Notifications.Core.Config;
using KTech.Notifications.Data.Messages;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace KTech.Notifications.Core.Actors
{
    public class NotificationWatcher : ReceiveActor
    {
        private readonly RabbitMqSettings _settings;

        private IConnection? _rabbitConnection { get; set; }
        private IModel? _rabbitChannel { get; set; }

        public NotificationWatcher(IOptions<RabbitMqSettings> settings)
        {
            _settings = settings.Value ?? throw new ArgumentNullException(nameof(RabbitMqSettings));

            Receive<AskForQueueMessageCount>(CheckQueueMessages);
        }

        protected override void PreStart()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.Username,
                Password = _settings.Password
            };

            _rabbitConnection = factory.CreateConnection();
            _rabbitChannel = _rabbitConnection.CreateModel();
        }

        protected override void PostStop()
        {
            _rabbitChannel?.Dispose();
            _rabbitConnection?.Dispose();
        }

        private void CheckQueueMessages(AskForQueueMessageCount message)
        {
            QueueDeclareOk? queue = _rabbitChannel?.QueueDeclare(
                queue: message.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            Sender.Tell(new TellQueueMessageCount(
                message.QueueName,
                queue?.MessageCount ?? 0));
        }
    }
}
