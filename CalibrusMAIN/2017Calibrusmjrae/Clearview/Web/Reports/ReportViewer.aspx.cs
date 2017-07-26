using Microsoft.Ajax.Utilities;
using Microsoft.Reporting.WebForms;
using System;
using System.Configuration;
using System.Web;

namespace Calibrus.ClearviewPortal.Web.Reports
{
	public partial class ReportViewer : System.Web.UI.Page
	{
		protected void Page_Init(object sender, EventArgs e)
		{
			if (!ReportAccess.IsAuthorized(HttpContext.Current, AccessLevel.OfficeAdministrator) || !Request.QueryString.HasKeys())
			{
				Response.Redirect("~/Error/AccessDenied", true);
			}

			ReportViewer1.ProcessingMode = ProcessingMode.Remote;
			ReportViewer1.AsyncRendering = false;
			ReportViewer1.SizeToReportContent = true;
			ReportViewer1.ZoomMode = ZoomMode.FullPage;

			IReportServerCredentials irsc = new CustomReportCredentials(ConfigurationManager.AppSettings["ReportUsername"],
				ConfigurationManager.AppSettings["ReportPassword"],
				ConfigurationManager.AppSettings["ReportDomain"]);
			ReportViewer1.ServerReport.ReportServerCredentials = irsc;

			ReportViewer1.ServerReport.ReportServerUrl = new Uri(ConfigurationManager.AppSettings["ReportServer"]);
			ReportViewer1.ServerReport.ReportPath = Request.QueryString["ReportPath"];

			ReportParameter vendorIdParameter = new ReportParameter { Name = "VendorId" };
			ReportParameter officeIdParameter = new ReportParameter { Name = "OfficeId" };

			if (SessionVars.LoggedInVendorId.HasValue && SessionVars.LoggedInVendorId.Value > 0)
			{
				vendorIdParameter.Values.Add(SessionVars.LoggedInVendorId.Value.ToString());
			}

			if (SessionVars.LoggedInOfficeId.HasValue && SessionVars.LoggedInOfficeId.Value > 0)
			{
				officeIdParameter.Values.Add(SessionVars.LoggedInOfficeId.Value.ToString());
			}

			ReportViewer1.ServerReport.SetParameters(new[] { vendorIdParameter, officeIdParameter });
			ReportViewer1.ServerReport.Refresh();
		}
	}
}