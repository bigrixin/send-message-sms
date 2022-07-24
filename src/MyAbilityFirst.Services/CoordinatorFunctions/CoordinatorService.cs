using AutoMapper;
using MyAbilityFirst.Domain;
using MyAbilityFirst.Infrastructure;
using MyAbilityFirst.Infrastructure.Data;
using MyAbilityFirst.Models;
using MyAbilityFirst.Services.ClientFunctions;
using MyAbilityFirst.Services.Common;
using System;

namespace MyAbilityFirst.Services.CoordinatorFunctions
{
	public class CoordinatorService : UserService, ICoordinatorService
	{

		#region Fields

		private readonly IWriteEntities _entities;
		private IMapper _mapper;
		private readonly INotificationService _notificationService;
		private readonly BookingData _bookingData;

		#endregion


		#region Ctor

		public CoordinatorService(IWriteEntities entities, IMapper mapper, INotificationService notificationService, BookingData bookingData) : base(entities, mapper)
		{
			this._entities = entities;
			this._mapper = mapper;
			this._notificationService = notificationService;
			this._bookingData = bookingData;
		}

		#endregion

		#region profile

		public CoordinatorDetailsViewModel GetCoordinatorVM(Coordinator currentCoordinator)
		{
			CoordinatorDetailsViewModel model = new CoordinatorDetailsViewModel();
			model = _mapper.Map<Coordinator, CoordinatorDetailsViewModel>(currentCoordinator);
			return model;
		}


		public void UpdateProfile(Coordinator updatedCoordinator)
		{
			updatedCoordinator.Status = UserStatus.Active;
			updatedCoordinator.UpdatedAt = DateTime.Now;
			this._entities.Update(updatedCoordinator);
			this._entities.Save();
		}

		#endregion

		#region review-approval

		public void ApproveRating(int coordinatorID, int ratingID)
		{
			Rating currentRating = this._entities.Single<Rating>(a => a.ID == ratingID);
			currentRating.Status = RatingStatus.Approved;
			currentRating.ApprovedDate = DateTime.Now;
			currentRating.CoordinatorID = coordinatorID;
			var booking = this._entities.Single<Booking>(b => b.ID == currentRating.BookingID);
			var careWorker = this._entities.Single<CareWorker>(c => c.ID == booking.CareWorkerID);
			careWorker.AddOverallRating(currentRating.OverallScore);

			this._entities.Update(careWorker);
			this._entities.Save();
		}

		public void ApproveCareWorker(int coordinatorID, int careWorkerID)
		{
			var careWorker = this._entities.Single<CareWorker>(c => c.ID == careWorkerID);
			if (careWorker == null)
				throw new ArgumentNullException("careWorker");
			careWorker.ApprovedDate = DateTime.Now;
			careWorker.CoordinatorID = coordinatorID;
			careWorker.Status = UserStatus.Active;

			this._entities.Update(careWorker);
			this._entities.Save();
		}

		#endregion


		#region sms

		public void SendMessageToMobile(int coordinatorID, SMSViewModel vm)
		{
			var coordinator = this._entities.Single<Coordinator>(a => a.ID == coordinatorID);
			string content = $"{vm.Content}, by {coordinator.FirstName} ";
			this._notificationService.SendASms(vm.MobileNumber, content);
		}

		#endregion
	}
}
