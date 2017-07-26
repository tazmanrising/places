using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using Elmah;

namespace Calibrus.SparkPortal.Web.CustomAttributes
{
    public class CustomHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            StringBuilder routeValues = new StringBuilder();
            foreach (var val in filterContext.RouteData.Values)
            {
                routeValues.AppendFormat(@"({0}:{1})/", val.Key, val.Value);
            }

            string url = "NULL";
            if (filterContext.HttpContext.Request.Url != null)
                url = filterContext.HttpContext.Request.Url.ToString();

            System.Configuration.Configuration config = WebConfigurationManager.OpenWebConfiguration("~/");
            CustomErrorsSection errorsSection = (CustomErrorsSection)config.GetSection("system.web/customErrors");

            if (errorsSection.Mode == CustomErrorsMode.On || (errorsSection.Mode == CustomErrorsMode.RemoteOnly && !HttpContext.Current.Request.IsLocal))
            {
                filterContext.ExceptionHandled = true;
                SingalElmah(filterContext);
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = "Index" }));
            }
            else
            {
                base.OnException(filterContext);
                SingalElmah(filterContext);
            }
        }

        private void SingalElmah(ExceptionContext filterContext)
        {
            var httpContext = filterContext.HttpContext.ApplicationInstance.Context;
            var signal = ErrorSignal.FromContext(httpContext);
            signal.Raise(filterContext.Exception, httpContext);
        }
    }
}