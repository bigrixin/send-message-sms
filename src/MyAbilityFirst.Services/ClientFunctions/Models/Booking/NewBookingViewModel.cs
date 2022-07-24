using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAbilityFirst.Domain.ClientFunctions
{
	public class NewBookingViewModel
	{

		public IEnumerable<NewBookingShortlistViewModel> Shortlist;

		[DisplayName("Whom are you requesting a service?")]
		[Required(ErrorMessage = "Please select a care worker")]
		public int CareWorkerID { get; set; }

		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMM yyyy hh:mm tt}")]
		[Required]
		public DateTime Start { get; set; }

		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd MMM yyyy hh:mm tt}")]
		[Required]
		public DateTime End { get; set; }

		[DisplayName("(Optional) Add a personal note")]
		public string Message { get; set; }

	}
}