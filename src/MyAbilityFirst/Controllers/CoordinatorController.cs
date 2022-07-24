using AutoMapper;
using MyAbilityFirst.Domain;
using MyAbilityFirst.Infrastructure.Auth;
using MyAbilityFirst.Infrastructure.Data;
using MyAbilityFirst.Models;
using MyAbilityFirst.Services.ClientFunctions;
using MyAbilityFirst.Services.CoordinatorFunctions;
using System;
using System.Web.Mvc;

namespace MyAbilityFirst.Controllers
{

	[Authorize(Roles = "Coordinator")]
	public class CoordinatorController : Controller
	{

		#region Fields

		private readonly IMapper _mapper;
		private readonly ICoordinatorService _coordinatorServices;
		private readonly BookingData _bookingData;
		private readonly INotificationService _notificationServices;

		#endregion

		#region Ctor

		public CoordinatorController(IMapper mapper, ICoordinatorService coordinatorServices, BookingData bookingData, INotificationService notificationServices)
		{
			this._mapper = mapper;
			this._coordinatorServices = coordinatorServices;
			this._bookingData = bookingData;
			this._notificationServices = notificationServices;
		}

		#endregion

		#region profile

		[Authorize(Roles = "Coordinator")]
		[HttpGet, Route("coordinator/myaccount")]
		public ActionResult MyAccount(string usertype)
		{
			var currentCoordinator = this.GetLoggedInUser() as Coordinator;
			CoordinatorDetailsViewModel model = this._coordinatorServices.GetCoordinatorVM(currentCoordinator);
			return View(model);
		}


		[Authorize(Roles = "Coordinator")]
		[HttpGet, Route("coordinator/editprofile")]
		public ActionResult EditProfile()
		{
			var currentCoordinator = this.GetLoggedInUser() as Coordinator;
			CoordinatorDetailsViewModel model = this._coordinatorServices.GetCoordinatorVM(currentCoordinator);
			return View(model);
		}

		[Authorize(Roles = "Coordinator")]
		[HttpPost, Route("coordinator/editprofile")]
		public ActionResult EditProfile(CoordinatorDetailsViewModel model)
		{
			if (ModelState.IsValid)
			{
				Coordinator updatedCoordinator = _mapper.Map(model, (Coordinator)this.GetLoggedInUser());
				this._coordinatorServices.UpdateProfile(updatedCoordinator);
			}
			return RedirectToAction("MyAccount/");
		}

		#endregion

		#region client-rating-review

		[HttpGet, Route("coordinator/ReviewRatings")]
		public ActionResult ReviewRatings()
		{
			var coordinator = this.GetLoggedInUser() as Coordinator;
			var bookingsVM = this._bookingData.GetBookingRatingsVMList(coordinator.ID);
			return View(bookingsVM);
		}

		[HttpGet, Route("coordinator/approverating/{ratingID:int}")]
		public ActionResult ApproveRating(int ratingID)
		{
			var coordinator = this.GetLoggedInUser() as Coordinator;
			this._coordinatorServices.ApproveRating(coordinator.ID, ratingID);
			return RedirectToAction("ReviewRatings");
		}

		#endregion

		#region careworker-review

		[HttpGet, Route("coordinator/reviewcareworkers")]
		public ActionResult ReviewCareWorkers()
		{
			var coordinator = this.GetLoggedInUser() as Coordinator;
			var careWorkersVM = this._bookingData.GetCareWorkerVMList(coordinator.ID);
			return View(careWorkersVM);
		}

		[HttpGet, Route("coordinator/approvecareworker/{CareWorkerID:int}")]
		public ActionResult ApproveCareWorker(int careWorkerID)
		{
			var coordinator = this.GetLoggedInUser() as Coordinator;
			this._coordinatorServices.ApproveCareWorker(coordinator.ID, careWorkerID);
			return RedirectToAction("ReviewCareWorkers");
		}

		#endregion

		#region sms

		[HttpGet, Route("coordinator/sms")]
		public ActionResult Sms()
		{
			SMSViewModel smsVM = new SMSViewModel();
			return View(smsVM);
		}

		[HttpPost, Route("coordinator/sms")]
		public ActionResult Sms(SMSViewModel vm)
		{
			var coordinator = this.GetLoggedInUser() as Coordinator;
			if (ModelState.IsValid)
				 this._coordinatorServices.SendMessageToMobile(coordinator.ID,vm);
			return RedirectToAction("sms");
		}


		#endregion
	}
}