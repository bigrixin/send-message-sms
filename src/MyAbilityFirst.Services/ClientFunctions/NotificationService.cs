using Mandrill;
using Mandrill.Models;
using Mandrill.Requests.Messages;
using Microsoft.AspNet.Identity;
using MyAbilityFirst.Domain;
using MyAbilityFirst.Infrastructure;
using MyAbilityFirst.Infrastructure.Auth;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace MyAbilityFirst.Services.ClientFunctions
{
	public class NotificationService : INotificationService
	{

		#region Fields

		private readonly IReadEntities _entities;
		private readonly AspNetIdentitySmsService _SmsServices;
		private const string smsInscribe = "@AbilityFirst,  Do Not Reply";
		#endregion

		#region Ctor

		public NotificationService(IReadEntities entities, AspNetIdentitySmsService smsServices)
		{
			this._entities = entities;
			this._SmsServices = smsServices;
		}

		#endregion

		#region email

		public void SendBookingRequestedEmail(Booking booking, string subject)
		{
			if (booking == null)
				throw new ArgumentNullException("booking");

			var client = this._entities.Single<User>(u => u.ID == booking.ClientID);
			var carer = this._entities.Single<User>(u => u.ID == booking.CareWorkerID);
			var context = EmailContext(booking, carer.FirstName, client.FirstName);
			var body = this.RenderPartialViewToString("~/Views/Email/Booking/BookingRequested.cshtml", context);
			var toEmailAddress = carer.Email;
			this.SendViaMandrill(subject, body, toEmailAddress);
		}

		public void SendBookingCancelledEmail(Booking booking, string subject)
		{
			if (booking == null)
				throw new ArgumentNullException("booking");

			var client = this._entities.Single<User>(u => u.ID == booking.ClientID);
			var carer = this._entities.Single<User>(u => u.ID == booking.CareWorkerID);
			var context = EmailContext(booking, carer.FirstName, client.FirstName);
			var body = this.RenderPartialViewToString("~/Views/Email/Booking/BookingCancelled.cshtml", context);
			var toEmailAddress = carer.Email;
			this.SendViaMandrill(subject, body, toEmailAddress);
		}

		public void SendBookingAcceptedEmail(Booking booking, string subject)
		{
			if (booking == null)
				throw new ArgumentNullException("booking");

			var client = this._entities.Single<User>(u => u.ID == booking.ClientID);
			var carer = this._entities.Single<User>(u => u.ID == booking.CareWorkerID);
			var context = EmailContext(booking, carer.FirstName, client.FirstName);
			var body = this.RenderPartialViewToString("~/Views/Email/Booking/BookingAccepted.cshtml", context);
			var toEmailAddress = client.Email;
			this.SendViaMandrill(subject, body, toEmailAddress);
		}

		public void SendBookingRejectedEmail(Booking booking, string subject)
		{
			if (booking == null)
				throw new ArgumentNullException("booking");

			var client = this._entities.Single<User>(u => u.ID == booking.ClientID);
			var carer = this._entities.Single<User>(u => u.ID == booking.CareWorkerID);
			var context = EmailContext(booking, carer.FirstName, client.FirstName);
			var body = this.RenderPartialViewToString("~/Views/Email/Booking/BookingRejected.cshtml", context);
			var toEmailAddress = client.Email;
			this.SendViaMandrill(subject, body, toEmailAddress);
		}

		public void SendBookingUpdatedByClientEmail(Booking booking, string subject)
		{
			if (booking == null)
				throw new ArgumentNullException("booking");

			var client = this._entities.Single<User>(u => u.ID == booking.ClientID);
			var carer = this._entities.Single<User>(u => u.ID == booking.CareWorkerID);
			var context = EmailContext(booking, carer.FirstName, client.FirstName);
			var body = this.RenderPartialViewToString("~/Views/Email/Booking/BookingUpdatedByClient.cshtml", context);
			var toEmailAddress = carer.Email;
			this.SendViaMandrill(subject, body, toEmailAddress);
		}

		public void SendBookingUpdatedByCareWorkerEmail(Booking booking, string subject)
		{
			if (booking == null)
				throw new ArgumentNullException("booking");

			var client = this._entities.Single<User>(u => u.ID == booking.ClientID);
			var carer = this._entities.Single<User>(u => u.ID == booking.CareWorkerID);
			var context = EmailContext(booking, carer.FirstName, client.FirstName);
			var body = this.RenderPartialViewToString("~/Views/Email/Booking/BookingUpdatedByCareWorker.cshtml", context);
			var toEmailAddress = client.Email;
			this.SendViaMandrill(subject, body, toEmailAddress);
		}

		public void SendBookingCompletedEmail(Booking booking, string subject)
		{
			if (booking == null)
				throw new ArgumentNullException("booking");

			var client = this._entities.Single<User>(u => u.ID == booking.ClientID);
			var carer = this._entities.Single<User>(u => u.ID == booking.CareWorkerID);
			var context = EmailContext(booking, carer.FirstName, client.FirstName);
			var body = this.RenderPartialViewToString("~/Views/Email/Booking/BookingCompleted.cshtml", context);
			var toEmailAddress = carer.Email;
			this.SendViaMandrill(subject, body, toEmailAddress);
		}

		public void SendBookingRatedEmail(Booking booking, string subject)
		{
			if (booking == null)
				throw new ArgumentNullException("booking");

			var client = this._entities.Single<User>(u => u.ID == booking.ClientID);
			var carer = this._entities.Single<User>(u => u.ID == booking.CareWorkerID);
			var context = EmailContext(booking, carer.FirstName, client.FirstName);
			var body = this.RenderPartialViewToString("~/Views/Email/Booking/BookingRated.cshtml", context);
			var toEmailAddress = carer.Email;
			this.SendViaMandrill(subject, body, toEmailAddress);
		}

		public void SendBookingRatingUpdatedEmail(Booking booking, string subject)
		{
			if (booking == null)
				throw new ArgumentNullException("booking");

			var client = this._entities.Single<User>(u => u.ID == booking.ClientID);
			var carer = this._entities.Single<User>(u => u.ID == booking.CareWorkerID);
			var context = EmailContext(booking, carer.FirstName, client.FirstName);
			var body = this.RenderPartialViewToString("~/Views/Email/Booking/BookingRatingUpdate.cshtml", context);
			var toEmailAddress = carer.Email;
			this.SendViaMandrill(subject, body, toEmailAddress);
		}

		#endregion

		#region sms

		public Task SendASms(string mobileNumber, string content)
		{
			IdentityMessage message = new IdentityMessage();
			message.Body = content + smsInscribe;
			message.Destination = mobileNumber;
			return this._SmsServices.SendAsync(message);
		}

		public List<string> SendBulkSms(Dictionary<string, string> people, string content)
		{
			List<string> result = new List<string>();
			foreach (var person in people)
			{
				string mobileNumber = person.Key;
				string name = person.Value;
				IdentityMessage message = new IdentityMessage();
				message.Body = content + smsInscribe;
				message.Destination = mobileNumber;
				if (this._SmsServices.SendAsync(message).ToString() != "success")
				{
					result.Add(mobileNumber);
				}
			}
			return result;
		}

		#endregion

		#region Private helpers

		private string RenderPartialViewToString(string templatePath, DynamicViewBag context)
		{
			string template = File.ReadAllText(HostingEnvironment.MapPath(templatePath));
			string renderedText = Engine.Razor.RunCompile(template, templatePath, null, context);
			return renderedText;
		}

		private DynamicViewBag EmailContext(Booking booking, string carerFirstName, string clientFirstName)
		{
			String strPathAndQuery = HttpContext.Current.Request.Url.PathAndQuery;
			String strUrl = HttpContext.Current.Request.Url.AbsoluteUri.Replace(strPathAndQuery, "/");
			var context = new DynamicViewBag();
			context.AddValue("CarerFirstName", carerFirstName);
			context.AddValue("ClientFirstName", clientFirstName);
			context.AddValue("BookingID", booking.ID);
			context.AddValue("BookingStart", booking.Schedule.Start.ToString("dd-MMM-yyyy"));
			context.AddValue("BookingEnd", booking.Schedule.End.ToString("dd-MMM-yyyy"));
			context.AddValue("Message", booking.Message);
			context.AddValue("CallbackURL", strUrl);
			return context;
		}

		private void SendViaMandrill(string subject, string body, string toEmailAddress)
		{
			var email = new EmailMessage
			{
				Subject = subject,
				Html = body,
				FromEmail = ConfigurationManager.AppSettings["mandrill.From"],
				FromName = ConfigurationManager.AppSettings["mandrill.FromName"],
				To = new[] { new EmailAddress(toEmailAddress) }
			};

			var task = Task.Run(async () =>
			{
				// TODO: (Prod) Need to configure Mandrill to allow for the new domain name
				var mandrillApi = new MandrillApi(ConfigurationManager.AppSettings["mandrill.ApiKey"]);
				var n = await mandrillApi.SendMessage(new SendMessageRequest(email));
				return n;
			});

			// To get result of the call say, for error logging, uncomment these lines below
			// task.Wait();
			// var result = task.Result;
		}

		#endregion

	}
}