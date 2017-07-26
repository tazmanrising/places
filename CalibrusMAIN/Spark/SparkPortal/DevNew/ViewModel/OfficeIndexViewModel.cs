using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Calibrus.SparkPortal.DataAccess.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace Calibrus.SparkPortal.ViewModel
{
	public class OfficeIndexViewModel
	{
		public OfficeIndexViewModel()
		{
			List<Office> offices = Business.AppLogic.GetOffices(false);
			Offices = new List<OfficeItem>();
			offices.ForEach(v =>
						Offices.Add(new OfficeItem()
						{
							Id = v.OfficeId,
							OfficeName = v.OfficeName,
							OfficeEmail = v.OfficeEmail,
                            MarketerCode = v.MarketerCode,
                            SalesChannel = v.SalesChannel.Name,
                            IsActive = v.IsActive,
							TotalMembers = v.Users.Count()
						}
					)
			);
		}

		public OfficeIndexViewModel(int id)
		{
			List<Office> offices = Business.AppLogic.GetOffices(false, id);
			Offices = new List<OfficeItem>();
			offices.ForEach(v =>
						Offices.Add(new OfficeItem()
						{
							Id = v.OfficeId,
							OfficeName = v.OfficeName,
							OfficeEmail = v.OfficeEmail,
                            MarketerCode = v.MarketerCode,
                            SalesChannel = v.SalesChannel.Name,
							IsActive = v.IsActive,
							TotalMembers = v.Users.Count()
						}
					)
			);
		}

		public List<OfficeItem> Offices { get; set; }

		public class OfficeItem
		{
			public int Id { get; set; }

			public string OfficeName { get; set; }

			public string OfficeEmail { get; set; }

            public string MarketerCode { get; set; }

            public string SalesChannel { get; set; }

            public bool IsActive { get; set; }

			public int TotalMembers { get; set; }
		}
	}
}