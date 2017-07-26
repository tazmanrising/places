using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QA_Reporting.Utilities
{
    public class CommonUtilities
    {
        public string DateHelper(DateTime d)
        {
            if (d.ToShortDateString() == "1/1/1901")
            {
                //return "";
                return "1/1/1753";
            }
            else if (d.ToShortDateString() == "1/1/0001")
            {
                //return "";
                return "1/1/1753";
            }
            else if (d.ToShortDateString() == "1/1/1753")
            {
                //return "";
                return "1/1/1753";
            }

            return d.ToShortDateString();
        }


        public static T ConvertFromDbVal<T>(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return default(T); // returns the default value for the type
            }
            else
            {
                return (T)obj;
            }
        }



    }

}