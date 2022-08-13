using KTech.Notifications.Core.Services.EmailService;
using KTech.Notifications.Core.Services.SmsService;
using System.Net;

namespace KTech.Notifications.Api.Controllers
{
    [ApiController]
    [Route("Notify")]
    public class NotifyController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;

        public NotifyController(
            IEmailService emailService, 
            ISmsService smsService
        )
        {
            _emailService = emailService;
            _smsService = smsService;
        }

        [HttpPost("Email")]
        public async Task<IActionResult> PublishEmail([FromBody] EmailData email)
        {
            HttpStatusCode? result = await _emailService.SendEmailAsync(email);
            return new OkObjectResult(result);
        }

        [HttpPost("SMS")]
        public async Task<IActionResult> PublishSMS([FromBody] SmsData sms)
        {
            string? uid = await _smsService.SendSmsAsync(sms);
            return new OkObjectResult(uid);
        }

        /*
        [HttpPost("Test")]
        public IActionResult Testing()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.Username,
                Password = _settings.Password,
            };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(
                        queue: _settings.NotificationQueue,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                    );

                    var message = new NotificationRequest
                    {
                        NotificationType = typeof(ScheduledEmailData),
                        NotificationData = new ScheduledEmailData
                        {
                            UtcScheduledTime = DateTimeOffset.UtcNow.AddMinutes(1),
                            To = new[] { "" },
                            From = "",
                            Subject = "RabbitMQ",
                            Body = "Scheduled Test"
                        }
                    };

                    var json = JsonConvert.SerializeObject(message);
                    var body = Encoding.UTF8.GetBytes(json);
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish(
                        exchange: "",
                        routingKey: _settings.NotificationQueue,
                        basicProperties: properties,
                        body: body
                    );
                }
            }

            return Ok();
        }

        [HttpPost("TestTwo")]
        public IActionResult TestingTwo()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.Username,
                Password = _settings.Password,
            };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(
                        queue: _settings.NotificationQueue,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                    );

                    var message = new NotificationRequest
                    {
                        NotificationType = typeof(EmailData),
                        NotificationData = new EmailData
                        {
                            To = new[] { "" },
                            From = "",
                            Subject = "RabbitMQ",
                            Body = "Scheduled Test"
                        }
                    };

                    var json = JsonConvert.SerializeObject(message);
                    var body = Encoding.UTF8.GetBytes(json);
                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish(
                        exchange: "",
                        routingKey: _settings.NotificationQueue,
                        basicProperties: properties,
                        body: body
                    );
                }
            }

            return Ok();
        }
        */
    }
}
