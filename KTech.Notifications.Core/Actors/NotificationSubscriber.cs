using Akka.Actor;
using Akka.DependencyInjection;
using Akka.Routing;
using KTech.Notifications.Core.Config;
using KTech.Notifications.Data.DTOs;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace KTech.Notifications.Core.Actors
{
    public class NotificationSubscriber : ReceiveActor
    {
        private readonly RabbitMqSettings _settings;

        private IConnection? _rabbitConnection { get; set; }
        private IModel? _rabbitChannel { get; set; }
        private EventingBasicConsumer? _consumer { get; set; }

        private IActorRef? _worker { get; set; }

        public NotificationSubscriber(IOptions<RabbitMqSettings> settings)
        {
            _settings = settings.Value ?? throw new ArgumentNullException(nameof(RabbitMqSettings));
        }

        protected override void PreStart()
        {
            Props workerProps = DependencyResolver.For(Context.System).Props<NotificationWorker>();
            _worker = Context.ActorOf(
                workerProps.WithRouter(new RoundRobinPool(_settings.WorkersPerSubscriber)), 
                $"NotificationWorker_{Guid.NewGuid()}");

            var factory = new ConnectionFactory()
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.Username,
                Password = _settings.Password
            };

            _rabbitConnection = factory.CreateConnection();
            _rabbitChannel = _rabbitConnection.CreateModel();

            _rabbitChannel?.QueueDeclare(
                queue: _settings.NotificationQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            _rabbitChannel?.BasicQos(
                prefetchSize: 0,
                prefetchCount: 1,
                global: false
            );

            _consumer = new EventingBasicConsumer(_rabbitChannel);
            _consumer.Received += HandleReceive;

            _rabbitChannel.BasicConsume(
                queue: _settings.NotificationQueue,
                autoAck: false,
                consumer: _consumer
            );
        }

        protected override void PostStop()
        {
            if (_consumer != null)
                _consumer.Received -= HandleReceive;

            _rabbitChannel?.Dispose();
            _rabbitConnection?.Dispose();
        }

        void HandleReceive(object? sender, BasicDeliverEventArgs args)
        {
            try
            {
                byte[] body = args.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);

                NotificationRequest? request = JsonConvert
                    .DeserializeObject<NotificationRequest>(message);

                if (request == null)
                    throw new ArgumentNullException(nameof(NotificationRequest));

                // TODO: Move Timespan to settings
                _worker.Ask(request, TimeSpan.FromSeconds(5))
                    .ContinueWith(result =>
                    {
                        if(result.Exception == null)
                            _rabbitChannel?.BasicAck(deliveryTag: args.DeliveryTag, multiple: false);
                        else
                        {
                            // TODO: Log Exception
                            _rabbitChannel?.BasicNack(
                                deliveryTag: args.DeliveryTag,
                                multiple: false,
                                requeue: false
                            );
                        }
                    });
            }
            catch (Exception ex)
            {
                // TODO: Log exception
                _rabbitChannel?.BasicNack(
                    deliveryTag: args.DeliveryTag,
                    multiple: false,
                    requeue: false
                );
            }
        }
    }
}
