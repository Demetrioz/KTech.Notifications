using Hangfire;
using KTech.Notifications.Core.Jobs;

namespace KTech.Notifications.Api.Controllers
{
    [ApiController]
    [Route("Schedule")]
    public class ScheduleController : ControllerBase
    {
        public ScheduleController()
        {
        }

        [HttpPost("Email")]
        public IActionResult ScheduleEmail([FromBody] ScheduledEmailData email)
        {
            if (email.UtcScheduledTime < DateTimeOffset.UtcNow)
                return new BadRequestObjectResult("Must supply a UtcScheduledTime in the future");

            string uid = BackgroundJob.Schedule<EmailJobs>(x => 
                x.ScheduleEmail(email), 
                email.UtcScheduledTime);

            return new OkObjectResult(uid);
        }

        [HttpPost("SMS")]
        public IActionResult ScheduleSMS([FromBody] ScheduledSmsData sms)
        {
            if (sms.UtcScheduledTime < DateTimeOffset.UtcNow)
                return new BadRequestObjectResult("Must supply a UtcScheduledTime in the future");

            string uid = BackgroundJob.Schedule<SmsJobs>(x =>
                x.ScheduleSMS(sms),
                DateTimeOffset.UtcNow.AddMinutes(1));

            return new OkObjectResult(uid);
        }
    }
}
