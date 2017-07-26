using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace HttpRecordingRedirector
{
	/// <summary>
	/// Summary description for CRR.
	/// </summary>
	public class CRR : IHttpHandler
	{
			
		#region IHttpHandler Members

		public void ProcessRequest(HttpContext ctx)
		{
			if(ctx.Request["recording"]==null||ctx.Request["recording"].ToString()=="")
			{
				ctx.Response.Write("<HTML>");
				ctx.Response.Write("<HEAD></HEAD>");
				ctx.Response.Write("<BODY>");
				ctx.Response.Write("<TABLE align='center' border='1' bordercolor='crimson' rules='none' cellpadding='20' cellspacing='0' bgcolor='gainsboro'>");
				ctx.Response.Write("<TR><TD><b>No Recording Name supplied.</b></TD></TR>");
				ctx.Response.Write("</TABLE>");
				ctx.Response.Write("</BODY>");
				ctx.Response.Write("</HTML>");
				ctx.Response.End();
			}
			else
			{
				Calibrus.Recordings.RecordingLocator rl = new Calibrus.Recordings.RecordingLocator(ctx.Request["recording"].ToString());
				if(rl.Archived==true)
				{
					ctx.Response.Write("<HTML>");
					ctx.Response.Write("<HEAD></HEAD>");
					ctx.Response.Write("<BODY>");
					ctx.Response.Write("<TABLE align='center' border='1' bordercolor='crimson' rules='none' cellpadding='20' cellspacing='0' bgcolor='gainsboro'>");
					ctx.Response.Write("<TR><TD><b>The recording you are looking for is not available.</b></TD></TR>");
					ctx.Response.Write("</TABLE>");
					ctx.Response.Write("</BODY>");
					ctx.Response.Write("</HTML>");
					ctx.Response.End();
				}
				else
				{
					ctx.Response.Redirect(@"https://recordings.calibrus.com/" + rl.RecordingUrl);
				}
			}
		}

		public bool IsReusable
		{
			get	{ return false;	}
		}


		#endregion

		

	}
}
