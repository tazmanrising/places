using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class E911_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ((Label)Master.FindControl("lblMasterTitle")).Text = "Frontier E911 Call Search";

        if (!IsPostBack)
        {
            Session.Abandon();
        }
    }
    private bool PerformValidation()
    {
        this.Validate();

        if (this.IsValid)
            return true;
        else
        {
            ((BulletedList)this.Master.FindControl("blErrorList")).Items.Clear();

            foreach (IValidator validationControl in this.Validators)
            {
                validationControl.Validate();

                if (!validationControl.IsValid)
                {
                    ((BulletedList)this.Master.FindControl("blErrorList")).Items.Add(validationControl.ErrorMessage);
                }
            }

            ((Label)this.Master.FindControl("lblErrorText")).Text = "You must correct the following errors before continuing.";
            return false;
        }
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        if (PerformValidation())
        {
            SessionVarsE911.SubscriberId = "";
            SessionVarsE911.Name = "";
            SessionVarsE911.PhoneNumber = "";
            SessionVarsE911.Disposition = "";
            SessionVarsE911.StartDate = null;
            SessionVarsE911.EndDate = null;

            ((Panel)this.Master.FindControl("pnlError")).Visible = false;
            if (!String.IsNullOrEmpty(txtSubscriberId.Text.Trim()))
            {
                SessionVarsE911.SubscriberId = txtSubscriberId.Text.Trim();
            }
            SessionVarsE911.PhoneNumber = txtPhoneNumber.Text.Trim();  
            SessionVarsE911.Name = txtName.Text.Trim();
            
            if (ddlConcern.SelectedIndex > 0)
            {
                SessionVarsE911.Disposition = ddlConcern.SelectedValue;
            }
            else
            {
                SessionVarsE911.Disposition = "";
            }
            if (!String.IsNullOrEmpty(txtStartDate.Text))
            {
                SessionVarsE911.StartDate = DateTime.Parse(txtStartDate.Text.Trim());
            }
            else
            {
                SessionVarsE911.StartDate = null;
            }

            if (!String.IsNullOrEmpty(txtEndDate.Text))
            {
                SessionVarsE911.EndDate = DateTime.Parse(txtEndDate.Text.Trim()).AddDays(1);
            }
            else
            {
                SessionVarsE911.EndDate = null;
            }

            SessionVarsE911.NewSearch = true;
            Response.Redirect("Results.aspx");
        }
        else
        {
            ((Panel)this.Master.FindControl("pnlError")).Visible = true;
        }
    }

    protected void custvSpecific_ServerValidate(object source, ServerValidateEventArgs args)
    {
        short subscriberId = 5;
        short phoneNumber = 5;
        short name = 2;
        short concern = 2;
        short shortDate = 5;
        short longDate = 3;
        short total = 0;

        if (!String.IsNullOrEmpty(txtSubscriberId.Text.Trim()))
            total += subscriberId;
        if (!String.IsNullOrEmpty(txtPhoneNumber.Text.Trim()))
            total += phoneNumber;
        if (!String.IsNullOrEmpty(txtName.Text.Trim()))
            total += name;
        if (!String.IsNullOrEmpty(ddlConcern.SelectedValue))
            total += concern;
        if (!String.IsNullOrEmpty(txtStartDate.Text.Trim()) && !String.IsNullOrEmpty(txtEndDate.Text.Trim()))
        {
            DateTime sDate = DateTime.Parse(txtStartDate.Text.Trim());
            DateTime eDate = DateTime.Parse(txtEndDate.Text.Trim());
            TimeSpan tsDiff = eDate.Subtract(sDate);
            if (tsDiff.Days <= 2)
            {
                total += shortDate;
            }
            else
            {
                total += longDate;
            }
        }

        if (total >= 5)
        {
            args.IsValid = true;
        }
        else
        {
            args.IsValid = false;
        }
    }
    protected void custvBothDates_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if ((!String.IsNullOrEmpty(txtStartDate.Text.Trim()) && String.IsNullOrEmpty(txtEndDate.Text.Trim())) ||
            (String.IsNullOrEmpty(txtStartDate.Text.Trim()) && !String.IsNullOrEmpty(txtEndDate.Text.Trim())))
        {
            args.IsValid = false;
        }
        else
        {
            args.IsValid = true;
        }
    }

    protected void custvDataRange_ServerValidate(object source, ServerValidateEventArgs args)
    {
        if (!String.IsNullOrEmpty(txtStartDate.Text.Trim()) && !String.IsNullOrEmpty(txtEndDate.Text.Trim()))
        {
            DateTime sDate = DateTime.Parse(txtStartDate.Text.Trim());
            DateTime eDate = DateTime.Parse(txtEndDate.Text.Trim());
            TimeSpan tsDiff = eDate.Subtract(sDate);
            if (tsDiff.Days > 30)
            {
                args.IsValid = false;
            }
            else
            {
                args.IsValid = true;
            }
        }
    }
}