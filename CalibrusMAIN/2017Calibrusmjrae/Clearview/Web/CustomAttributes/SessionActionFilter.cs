using System.Web.Mvc;

namespace Calibrus.ClearviewPortal.Web.CustomAttributes
{
    public class SessionExpireAttribute : ActionFilterAttribute
    {
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			// check  sessions here
			if (SessionVars.UserName == null)
			{
				filterContext.Result = new RedirectResult("~/Account/Login");
				return;
			}

			base.OnActionExecuting(filterContext);
		}
    }
}