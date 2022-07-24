using System.ComponentModel.DataAnnotations;

namespace MyAbilityFirst.Domain.ClientFunctions
{
	public class UpdateBookingViewModel : NewBookingViewModel
	{

		[Required]
		public int BookingID { get; set; }

		public bool IsCancelled { get; set; }

		public bool IsLapsed { get; set; }

	}
}