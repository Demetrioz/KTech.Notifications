using Akka.Actor;
using KTech.Notifications.Core.Config;
using KTech.Notifications.Data.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace KTech.Notifications.Core.Actors
{
    public class NotificationManager : ReceiveActor
    {
        private readonly IServiceScope _scope;
        private readonly IOptions<RabbitMqSettings> _options;

        private IActorRef? _queueWatcher { get; set; }
        private List<IActorRef> _subscribers { get; set; } = new List<IActorRef>();

        public NotificationManager(IServiceProvider provider)
        {
            _scope = provider.CreateScope();
            _options = _scope.ServiceProvider
                .GetRequiredService<IOptions<RabbitMqSettings>>();

            Receive<TellQueueMessageCount>(HandleQueueMessageCount);
        }

        protected override void PreStart()
        {
            RabbitMqSettings settings = _options.Value;

            Props watcherProps = Props.Create(() => new NotificationWatcher(_options));
            _queueWatcher = Context.ActorOf(watcherProps, "NotificationWatcher");

            Props subscriberProps = Props.Create(() => new NotificationSubscriber(_options));
            _subscribers.Add(Context.ActorOf(subscriberProps, $"NotificationSubscriber_{Guid.NewGuid()}"));

            Context.System.Scheduler.ScheduleTellRepeatedly(
                TimeSpan.Zero,
                TimeSpan.FromMinutes(settings.QueueCheckIntervalMinutes),
                _queueWatcher,
                new AskForQueueMessageCount(settings.NotificationQueue),
                Self
            );
        }

        protected override void PostStop()
        {
            _scope.Dispose();
        }

        private void HandleQueueMessageCount(TellQueueMessageCount message)
        {
            // TODO: Implement scaling logic
        }
    }
}
