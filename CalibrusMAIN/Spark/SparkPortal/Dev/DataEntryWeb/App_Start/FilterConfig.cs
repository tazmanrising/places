using System.Web;
using System.Web.Mvc;

namespace Calibrus.SparkPortal.DataEntryWeb
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
