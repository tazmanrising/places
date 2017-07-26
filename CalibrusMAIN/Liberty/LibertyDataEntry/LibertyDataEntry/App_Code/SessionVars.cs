using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LibertyModel;

/// <summary>
/// Summary description for SessionVars
/// </summary>
public class SessionVars
{
    #region EF Classes
    public static tblUserLogin AdminUser
    {
        get
        {
            if (HttpContext.Current.Session["AdminUser"] == null)
                return null;
            else
                return (tblUserLogin)HttpContext.Current.Session["AdminUser"];
        }
        set
        {
            if (HttpContext.Current.Session["AdminUser"] == null)
                HttpContext.Current.Session.Add("AdminUser", value);
            else
                HttpContext.Current.Session["AdminUser"] = value;
        }
    }

    public static Vendor Vendor
    {
        get
        {
            if (HttpContext.Current.Session["Vendor"] == null)
                return null;
            else
                return (Vendor)HttpContext.Current.Session["Vendor"];
        }
        set
        {
            if (HttpContext.Current.Session["Vendor"] == null)
                HttpContext.Current.Session.Add("Vendor", value);
            else
                HttpContext.Current.Session["Vendor"] = value;
        }
    }

    public static Office Office
    {
        get
        {
            if (HttpContext.Current.Session["Office"] == null)
                return null;
            else
                return (Office)HttpContext.Current.Session["Office"];
        }
        set
        {
            if (HttpContext.Current.Session["Office"] == null)
                HttpContext.Current.Session.Add("Office", value);
            else
                HttpContext.Current.Session["Office"] = value;
        }
    }

    public static SalesChannel SalesChannel
    {
        get
        {
            if (HttpContext.Current.Session["SalesChannel"] == null)
                return null;
            else
                return (SalesChannel)HttpContext.Current.Session["SalesChannel"];
        }
        set
        {
            if (HttpContext.Current.Session["SalesChannel"] == null)
                HttpContext.Current.Session.Add("SalesChannel", value);
            else
                HttpContext.Current.Session["SalesChannel"] = value;
        }
    }
    //Used to hold submitted Account after Submit
    public static Main CurrentAccount
    {
        get
        {
            if (HttpContext.Current.Session["CurrentAccount"] == null)
                return null;
            else
                return (Main)HttpContext.Current.Session["CurrentAccount"];
        }
        set
        {
            if (HttpContext.Current.Session["CurrentAccount"] == null)
                HttpContext.Current.Session.Add("CurrentAccount", value);
            else
                HttpContext.Current.Session["CurrentAccount"] = value;
        }
    }

    public static Main MainRecord
    {
        get
        {
            if (HttpContext.Current.Session["MainRecord"] == null)
                return null;
            else
                return (Main)HttpContext.Current.Session["MainRecord"];
        }
        set
        {
            if (HttpContext.Current.Session["MainRecord"] == null)
                HttpContext.Current.Session.Add("MainRecord", value);
            else
                HttpContext.Current.Session["MainRecord"] = value;
        }
    }
    public static OrderDetail OrderDetailRecord
    {
        get
        {
            if (HttpContext.Current.Session["OrderDetailRecord"] == null)
                return null;
            else
                return (OrderDetail)HttpContext.Current.Session["OrderDetailRecord"];
        }
        set
        {
            if (HttpContext.Current.Session["OrderDetailRecord"] == null)
                HttpContext.Current.Session.Add("OrderDetailRecord", value);
            else
                HttpContext.Current.Session["OrderDetailRecord"] = value;
        }
    }
    #endregion

    #region User Created Classes
    //used to determine if the ModalForm will be in Edit or Add mode
    public static bool AccountEditMode
    {
        get
        {
            if (HttpContext.Current.Session["AccountEditMode"] == null)
                return false;
            else
                return (bool)HttpContext.Current.Session["AccountEditMode"];
        }
        set
        {
            if (HttpContext.Current.Session["AccountEditMode"] == null)
                HttpContext.Current.Session.Add("AccountEditMode", value);
            else
                HttpContext.Current.Session["AccountEditMode"] = value;
        }
    }

    //used to link Number OrderDetailFormRecords internally to keep track of which came first and which is to be edited
    public static int OrderDetailFormRecordNumber
    {
        get
        {

            if (HttpContext.Current.Session["OrderDetailFormRecordNumber"] == null)
                return 0;
            else
                return (int)HttpContext.Current.Session["OrderDetailFormRecordNumber"];
        }

        set
        {
            if (HttpContext.Current.Session["OrderDetailFormRecordNumber"] == null)
                HttpContext.Current.Session.Add("OrderDetailFormRecordNumber", value);
            else
                HttpContext.Current.Session["OrderDetailFormRecordNumber"] = value;
        }
    }

    //User Classes to hold data before submitting to the DB
    public static MainFormRecord MainFormRecord
    {
        get
        {
            if (HttpContext.Current.Session["MainFormRecord"] == null)
                return null;
            else
                return (MainFormRecord)HttpContext.Current.Session["MainFormRecord"];
        }
        set
        {
            if (HttpContext.Current.Session["MainFormRecord"] == null)
                HttpContext.Current.Session.Add("MainFormRecord", value);
            else
                HttpContext.Current.Session["MainFormRecord"] = value;
        }
    }
    public static OrderDetailFormRecord OrderDetailFormRecord
    {
        get
        {
            if (HttpContext.Current.Session["OrderDetailFormRecord"] == null)
                return null;
            else
                return (OrderDetailFormRecord)HttpContext.Current.Session["OrderDetailFormRecord"];
        }
        set
        {
            if (HttpContext.Current.Session["OrderDetailFormRecord"] == null)
                HttpContext.Current.Session.Add("OrderDetailFormRecord", value);
            else
                HttpContext.Current.Session["OrderDetailFormRecord"] = value;
        }
    }
    public static List<OrderDetailFormRecord> OrderDetailFormRecordList
    {
        get
        {
            if (HttpContext.Current.Session["OrderDetailFormRecordList"] == null)
                return null;
            else
                return (List<OrderDetailFormRecord>)HttpContext.Current.Session["OrderDetailFormRecordList"];
        }
        set
        {
            if (HttpContext.Current.Session["OrderDetailFormRecordList"] == null)
                HttpContext.Current.Session.Add("OrderDetailFormRecordList", value);
            else
                HttpContext.Current.Session["OrderDetailFormRecordList"] = value;
        }
    }
    #endregion

    


}