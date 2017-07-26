using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FrontierModel;

public partial class PhoneRecord : System.Web.UI.UserControl
{

    public bool ShowHeader { get { return serviceHeading.Visible; } set { serviceHeading.Visible = value; } }
    public bool TnRequired { get { return rfPhoneNumber.Enabled; } set { rfPhoneNumber.Enabled = value; } }

    protected void Page_Load(object sender, EventArgs e)
    {
        serviceHeading.Visible = ShowHeader;
    }

    public tblTn GetTn()
    {
        tblTn tn = new tblTn();
        tn.Tn = txtPhoneNumber.Text;
        if (String.IsNullOrEmpty(tn.Tn))
            return null;
        tn.DialTone = chkLocal.Checked ? "1" : "0";
        tn.LocalToll = chkIntralata.Checked ? "1" : "0";
        tn.Ld = chkInterlata.Checked ? "1" : "0";
        tn.DialToneFreeze = chkLocalFreeze.Checked ? "1" : "0";
        tn.LocalTollFreeze = chkIntralataFreeze.Checked ? "1" : "0";
        tn.LdFreeze = chkInterlataFreeze.Checked ? "1" : "0";

        return (tn);
    }

    protected void cvEmtyOrder_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (!String.IsNullOrEmpty(txtPhoneNumber.Text))
        {
            if (!chkLocal.Checked && !chkIntralata.Checked && !chkInterlata.Checked
                && !chkLocalFreeze.Checked && !chkIntralataFreeze.Checked && !chkInterlataFreeze.Checked)
            {
                args.IsValid = false;
            }
            else
            {
                args.IsValid = true;
            }
        }
        else
        {
            args.IsValid = true;
        }
    }
    
}