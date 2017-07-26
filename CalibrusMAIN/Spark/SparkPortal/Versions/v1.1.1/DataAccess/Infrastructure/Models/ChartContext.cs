﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calibrus.SparkPortal.DataAccess.Models
{
	public class ChartContext
	{
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public int? VendorId { get; set; }
		public int? OfficeId { get; set; }
	}
}
