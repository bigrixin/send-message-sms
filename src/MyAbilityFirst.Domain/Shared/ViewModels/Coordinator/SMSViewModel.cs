using System;
using System.ComponentModel.DataAnnotations;

namespace MyAbilityFirst.Models
{
	public class SMSViewModel
	{
		public int ID { get; set; }
		public int ClientID { get; set; }
		public int CareWorkerID { get; set; }
		public int CoordinatorID { get; set; }
		public string ClientFirstName { get; set; }
		public string CareWorkerFirstName { get; set; }
		[Display(Name = "Mobile")]
		[DataType(DataType.PhoneNumber)]
		[Required(ErrorMessage = "Your must provide a PhoneNumber")]
		[RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Not a valid Phone number")]
		public string MobileNumber { get; set; }
		[DataType(DataType.Text)]
		[Required]
		[StringLength(50, MinimumLength = 2, ErrorMessage = "Use 2-50 characters")]
		public string Content { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}
