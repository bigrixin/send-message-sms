using MyAbilityFirst.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyAbilityFirst.Services.ClientFunctions
{
	public interface INotificationService
	{
		void SendBookingRequestedEmail(Booking booking, string subject);
		void SendBookingCancelledEmail(Booking booking, string subject);
		void SendBookingAcceptedEmail(Booking booking, string subject);
		void SendBookingRejectedEmail(Booking booking, string subject);
		void SendBookingUpdatedByClientEmail(Booking booking, string subject);
		void SendBookingUpdatedByCareWorkerEmail(Booking booking, string subject);
		void SendBookingCompletedEmail(Booking booking, string subject);
		void SendBookingRatedEmail(Booking booking, string subject);
		void SendBookingRatingUpdatedEmail(Booking booking, string subject);
		Task SendASms(string mobileNumber, string content);
		List<string> SendBulkSms(Dictionary<string, string> people, string content);
	}
}