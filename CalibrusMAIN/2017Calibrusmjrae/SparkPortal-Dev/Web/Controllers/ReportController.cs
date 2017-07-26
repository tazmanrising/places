using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Calibrus.SparkPortal.Web.CustomAttributes;

namespace Calibrus.SparkPortal.Web.Controllers
{
	[CustomAccessAuthorize(AccessLevel = AccessLevel.OfficeAdministrator)]
    public class ReportController : Controller
    {
        // GET: Report
        public ActionResult CallReport()
        {
            return View();
        }
    }
}