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
public class SessionVarsE911
{  

    public static string SubscriberId
    {
        get
        {
            if (HttpContext.Current.Session["SubscriberId"] == null)
                return null;
            else
                return HttpContext.Current.Session["SubscriberId"].ToString();
        }
        set
        {
            if (HttpContext.Current.Session["SubscriberId"] == null)
                HttpContext.Current.Session.Add("SubscriberId", value);
            else
                HttpContext.Current.Session["SubscriberId"] = value;
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

    public static string Name
    {
        get
        {
            if (HttpContext.Current.Session["Name"] == null)
                return null;
            else
                return HttpContext.Current.Session["Name"].ToString();
        }
        set
        {
            if (HttpContext.Current.Session["Name"] == null)
                HttpContext.Current.Session.Add("Name", value);
            else
                HttpContext.Current.Session["Name"] = value;
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
