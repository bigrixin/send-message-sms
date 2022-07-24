using Microsoft.AspNet.Identity;
using System.Configuration;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace MyAbilityFirst.Infrastructure.Auth
{
	public class AspNetIdentitySmsService : IIdentityMessageService
	{

		public Task SendAsync(IdentityMessage message)
		{
			string accountID = ConfigurationManager.AppSettings["Twilio_SmsAccount_ID"];
			string authToken = ConfigurationManager.AppSettings["Twilio_SmsAccount_Token"];
			string fromPhoneNumber = ConfigurationManager.AppSettings["Twilio_SmsFrom_PhoneNumber"];

			// Initialize the Twilio client
			TwilioClient.Init(accountID, authToken);

			var result = MessageResource.Create(
					from: new PhoneNumber(fromPhoneNumber),
					to: new PhoneNumber(message.Destination),
					body: message.Body);
			return Task.FromResult(0);
		}

	}
}