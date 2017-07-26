using Calibrus.SparkPortal.DataAccess.Infrastructure;
using System;

namespace Calibrus.SparkPortal.DataAccess.Models
{
	public class SearchContext
	{
		public int? RecordId { get; set; }

		public string VerificationCode { get; set; }

		public string PhoneNumber { get; set; }

		public string AccountNumber { get; set; }

		public string VendorAgentId { get; set; }

		public string TpvAgentId { get; set; }

		public DateTime? StartDate { get; set; }

		public DateTime? EndDate { get; set; }

		public Disposition Disposition { get; set; }

		public int? VendorId { get; set; }

		public int? OfficeId { get; set; }
	}
}