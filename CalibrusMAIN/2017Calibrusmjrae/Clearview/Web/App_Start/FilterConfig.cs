using System.Web.Mvc;
using Calibrus.ClearviewPortal.Web.CustomAttributes;

namespace Calibrus.ClearviewPortal.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandleErrorAttribute());
            filters.Add(new CustomHandleErrorAttribute());
        }
    }
}