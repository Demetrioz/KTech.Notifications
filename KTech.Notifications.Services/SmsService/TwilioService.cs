using KTech.Notifications.Core.Config;
using KTech.Notifications.Data.DTOs;
using Microsoft.Extensions.Options;
using Twilio.Clients;
using Twilio.Http;
using Twilio.Rest.Api.V2010.Account;
using HttpClient = System.Net.Http.HttpClient;

namespace KTech.Notifications.Services.SmsService
{
    public class TwilioService : ISmsService, ITwilioRestClient
    {
        private readonly ITwilioRestClient _client;

        public string AccountSid => _client.AccountSid;
        public string Region => _client.Region;
        public Twilio.Http.HttpClient HttpClient => _client.HttpClient;
        public Response Request(Request request) => _client.Request(request);
        public Task<Response> RequestAsync(Request request) => _client.RequestAsync(request);

        public TwilioService(IOptions<TwilioSettings> settings, HttpClient httpClient)
        {
            if (httpClient == null)
                throw new ArgumentNullException(nameof(HttpClient));

            if (settings == null)
                throw new ArgumentNullException(nameof(TwilioSettings));

            _client = new TwilioRestClient(
                settings.Value.SID,
                settings.Value.Token,
                httpClient: new SystemNetHttpClient(httpClient)
            );
        }

        public async Task<string?> SendSmsAsync(SmsData sms)
        {
            // TODO: Handle errors
            // TODO: Handle failures / retries

            MessageResource? message = await MessageResource.CreateAsync(
                to: new Twilio.Types.PhoneNumber(sms.To),
                from: new Twilio.Types.PhoneNumber(sms.From),
                body: sms.Message,
                client: _client
            );

            return message?.Sid;
        }
    }
}
