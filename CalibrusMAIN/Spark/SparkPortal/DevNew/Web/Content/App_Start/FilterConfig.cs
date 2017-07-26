using System.Web.Mvc;
using Calibrus.SparkPortal.Web.CustomAttributes;

namespace Calibrus.SparkPortal.Web
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