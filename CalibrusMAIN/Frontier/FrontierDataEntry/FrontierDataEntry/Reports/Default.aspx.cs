using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Reports_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ((Label)Master.FindControl("lblMasterTitle")).Text = "Frontier Call Search";

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
            SessionVars.RecordLocator = 0;
            SessionVars.PhoneNumber = "";
            SessionVars.TpvAgentId = "";
            SessionVars.BillingName = "";
            SessionVars.CompanyName = "";
            SessionVars.Disposition = "";
            SessionVars.StartDate = null;
            SessionVars.EndDate = null;

            ((Panel)this.Master.FindControl("pnlError")).Visible = false;
            if (!String.IsNullOrEmpty(txtRecordLocator.Text.Trim()))
            {
                SessionVars.RecordLocator = int.Parse(txtRecordLocator.Text.Trim());
            }
            SessionVars.PhoneNumber = txtPhoneNumber.Text.Trim();
            SessionVars.TpvAgentId = txtTpvAgentId.Text.Trim();
            SessionVars.BillingName = txtBillingName.Text.Trim();
            SessionVars.CompanyName = txtCompanyName.Text.Trim();
            if (ddlConcern.SelectedIndex > 0)
            {
                SessionVars.Disposition = ddlConcern.SelectedValue;
            }
            else
            {
                SessionVars.Disposition = "";
            }
            if (!String.IsNullOrEmpty(txtStartDate.Text))
            {
                SessionVars.StartDate = DateTime.Parse(txtStartDate.Text.Trim());
            }
            else
            {
                SessionVars.StartDate = null;
            }

            if (!String.IsNullOrEmpty(txtEndDate.Text))
            {
                SessionVars.EndDate = DateTime.Parse(txtEndDate.Text.Trim()).AddDays(1);
            }
            else
            {
                SessionVars.EndDate = null;
            }

            SessionVars.NewSearch = true;
            Response.Redirect("Results.aspx");
        }
        else
        {
            ((Panel)this.Master.FindControl("pnlError")).Visible = true;
        }
    }

    protected void custvSpecific_ServerValidate(object source, ServerValidateEventArgs args)
    {
        short recordLocator = 5;
        short phoneNumber = 5;
        short tpvAgentId = 2;
        short billingName = 2;
        short companyName = 2;
        short concern = 2;
        short shortDate = 5;
        short longDate = 3;
        short total = 0;

        if (!String.IsNullOrEmpty(txtRecordLocator.Text.Trim()))
            total += recordLocator;
        if (!String.IsNullOrEmpty(txtPhoneNumber.Text.Trim()))
            total += phoneNumber;
        if (!String.IsNullOrEmpty(txtTpvAgentId.Text.Trim()))
            total += tpvAgentId;
        if (!String.IsNullOrEmpty(txtBillingName.Text.Trim()))
            total += billingName;
        if (!String.IsNullOrEmpty(txtCompanyName.Text.Trim()))
            total += companyName;
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