﻿using MyAbilityFirst.Domain;
using MyAbilityFirst.Models;
using System.Collections.Generic;

namespace MyAbilityFirst.Services.CoordinatorFunctions
{
	public interface ICoordinatorService
	{
		CoordinatorDetailsViewModel GetCoordinatorVM(Coordinator currentCoordinator);
		void UpdateProfile(Coordinator updatedCoordinator);
		void ApproveRating(int coordinatorID, int ratingID);
		void ApproveCareWorker(int coordinatorID, int careWorkerID);
		void SendMessageToMobile(int coordinatorID, SMSViewModel vm);
	}
}
