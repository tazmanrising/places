using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FrontierModel;

/// <datamodel>
/// using SBCModel;
/// </datamodel>

/// <summary>
/// Summary description for SessionVars
/// </summary>
public class SessionVars
{

    public static tblMain CurrentAccount
    {
        get
        {
            if (HttpContext.Current.Session["CurrentAccount"] == null)
                return null;
            else
                return (tblMain)HttpContext.Current.Session["CurrentAccount"];
        }
        set
        {
            if (HttpContext.Current.Session["CurrentAccount"] == null)
                HttpContext.Current.Session.Add("CurrentAccount", value);
            else
                HttpContext.Current.Session["CurrentAccount"] = value;
        }
    }
    

    public static int RecordLocator
    {
        get
        {
            if (HttpContext.Current.Session["RecordLocator"] == null)
                return 0;
            else
                return (int)HttpContext.Current.Session["RecordLocator"];
        }
        set
        {
            if (HttpContext.Current.Session["RecordLocator"] == null)
                HttpContext.Current.Session.Add("RecordLocator", value);
            else
                HttpContext.Current.Session["RecordLocator"] = value;
        }
    }

    public static string PhoneNumberList
    {
        get
        {
            if (HttpContext.Current.Session["PhoneNumberList"] == null)
                return null;
            else
                return HttpContext.Current.Session["PhoneNumberList"].ToString();
        }
        set
        {
            if (HttpContext.Current.Session["PhoneNumberList"] == null)
                HttpContext.Current.Session.Add("PhoneNumberList", value);
            else
                HttpContext.Current.Session["PhoneNumberList"] = value;
        }
    }

    public static string PhoneNumber
    {
        get
        {
            if (HttpContext.Current.Session["PhoneNumber"] == null)
                return null;
            else
                return HttpContext.Current.Session["PhoneNumber"].ToString();
        }
        set
        {
            if (HttpContext.Current.Session["PhoneNumber"] == null)
                HttpContext.Current.Session.Add("PhoneNumber", value);
            else
                HttpContext.Current.Session["PhoneNumber"] = value;
        }
    }

    public static string TpvAgentId
    {
        get
        {
            if (HttpContext.Current.Session["TpvAgentId"] == null)
                return null;
            else
                return HttpContext.Current.Session["TpvAgentId"].ToString();
        }
        set
        {
            if (HttpContext.Current.Session["TpvAgentId"] == null)
                HttpContext.Current.Session.Add("TpvAgentId", value);
            else
                HttpContext.Current.Session["TpvAgentId"] = value;
        }
    }

    public static string SalesAgentId
    {
        get
        {
            if (HttpContext.Current.Session["SalesAgentId"] == null)
                return null;
            else
                return HttpContext.Current.Session["SalesAgentId"].ToString();
        }
        set
        {
            if (HttpContext.Current.Session["SalesAgentId"] == null)
                HttpContext.Current.Session.Add("SalesAgentId", value);
            else
                HttpContext.Current.Session["SalesAgentId"] = value;
        }
    }

    public static string BillingName
    {
        get
        {
            if (HttpContext.Current.Session["BillingName"] == null)
                return null;
            else
                return HttpContext.Current.Session["BillingName"].ToString();
        }
        set
        {
            if (HttpContext.Current.Session["BillingName"] == null)
                HttpContext.Current.Session.Add("BillingName", value);
            else
                HttpContext.Current.Session["BillingName"] = value;
        }
    }

    public static string CompanyName
    {
        get
        {
            if (HttpContext.Current.Session["CompanyName"] == null)
                return null;
            else
                return HttpContext.Current.Session["CompanyName"].ToString();
        }
        set
        {
            if (HttpContext.Current.Session["CompanyName"] == null)
                HttpContext.Current.Session.Add("CompanyName", value);
            else
                HttpContext.Current.Session["CompanyName"] = value;
        }
    }

    public static string Disposition
    {
        get
        {
            if (HttpContext.Current.Session["Disposition"] == null)
                return null;
            else
                return HttpContext.Current.Session["Disposition"].ToString();
        }
        set
        {
            if (HttpContext.Current.Session["Disposition"] == null)
                HttpContext.Current.Session.Add("Disposition", value);
            else
                HttpContext.Current.Session["Disposition"] = value;
        }
    }

    public static DateTime? StartDate
    {
        get
        {
            if (HttpContext.Current.Session["StartDate"] == null)
                return null;
            else
                return (DateTime?)HttpContext.Current.Session["StartDate"];
        }
        set
        {
            if (HttpContext.Current.Session["StartDate"] == null)
                HttpContext.Current.Session.Add("StartDate", value);
            else
                HttpContext.Current.Session["StartDate"] = value;
        }
    }

    public static DateTime? EndDate
    {
        get
        {
            if (HttpContext.Current.Session["EndDate"] == null)
                return null;
            else
                return (DateTime?)HttpContext.Current.Session["EndDate"];
        }
        set
        {
            if (HttpContext.Current.Session["EndDate"] == null)
                HttpContext.Current.Session.Add("EndDate", value);
            else
                HttpContext.Current.Session["EndDate"] = value;
        }
    }

    public static bool NewSearch
    {
        get
        {
            if (HttpContext.Current.Session["NewSearch"] == null)
                return false;
            else
                return (bool)HttpContext.Current.Session["NewSearch"];
        }
        set
        {
            if (HttpContext.Current.Session["NewSearch"] == null)
                HttpContext.Current.Session.Add("NewSearch", value);
            else
                HttpContext.Current.Session["NewSearch"] = value;
        }
    }


}
