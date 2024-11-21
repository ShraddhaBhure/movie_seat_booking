using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Exceptions;
using Microsoft.Extensions.Configuration;


namespace movie_seat_booking.Services
{
    public class SmsService
    {
        private readonly IConfiguration _configuration;

        public SmsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendSms(string toPhoneNumber, string message)
        {
            var accountSid = _configuration["Twilio:AccountSid"];
            var authToken = _configuration["Twilio:AuthToken"];
            var fromPhoneNumber = _configuration["Twilio:FromPhoneNumber"];

            TwilioClient.Init(accountSid, authToken);

            try
            {
                var messageResource = MessageResource.Create(
                    body: message,
                    from: new Twilio.Types.PhoneNumber(fromPhoneNumber),
                    to: new Twilio.Types.PhoneNumber(toPhoneNumber)
                );
            }
            catch (ApiException ex)
            {
                // Handle error in case of failed SMS sending
                Console.WriteLine($"Error sending SMS: {ex.Message}");
            }
        }
    }
}
