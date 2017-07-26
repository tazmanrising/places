using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FrontierModel;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Session.Abandon();
        }
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        if (PerformValidation())
        {
            ((Panel)this.Master.FindControl("pnlError")).Visible = false;
            SessionVars.CurrentAccount = InsertRecord();
            Response.Redirect("Display.aspx");
        }
        else
        {
            ((Panel)this.Master.FindControl("pnlError")).Visible = true;
        }
    }

    private tblMain InsertRecord()
    {
        tblMain main = null;

        using (FrontierEntities data = new FrontierEntities())
        {
            main = new tblMain();
            
            main.SalesAgentId = txtAgentId.Text;
            main.State = ddlState.Value;
            main.CustFirstName = txtBillingFirstName.Text;
            main.CustLastName = txtBillingLastName.Text;
            main.DecisionMaker = String.Format("{0} {1}", txtCustomerFirstName.Text, txtCustomerLastName.Text);
            main.CompanyName = txtCompanyName.Text;
            main.Product = txtProduct.Text;
            main.Business = rbAccountTypeBusiness.Checked ? "1" : "0";

            for (int x = 1; x <= 10; x++)
            {
                PhoneRecord phone = FindControl<PhoneRecord>("Tn" + x.ToString());
                tblTn tn = phone.GetTn();

                if (tn != null)
                {
                    main.tblTns.Add(tn);
                }
            }

            data.AddTotblMains(main);
            data.SaveChanges();

            SessionVars.PhoneNumberList = main.PhoneNumberList;
        }

        return main;
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
                if (!validationControl.IsValid)
                {
                    ((BulletedList)this.Master.FindControl("blErrorList")).Items.Add(validationControl.ErrorMessage);
                }
            }

            ((Label)this.Master.FindControl("lblErrorText")).Text = "You must correct the following errors before continuing.";
            return false;
        }
    }

    protected void cvAccountTYpe_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = rbAccountTypeBusiness.Checked || rbAccountTypeResidential.Checked;
    }

    // <summary> 
    /// Attempts to find control of given type within current page where given ID matches.

    /// </summary> 
    /// <typeparam name="T">Type of control to find</typeparam> 
    /// <param name="id">ID of control to find</param> 
    /// <returns>Control if found; null if not.</returns> 
    public static T FindControl<T>(string id) where T : Control
    {
        return FindControl<T>(HttpContext.Current.Handler as Page, id);
    }


    /// <summary> 
    /// Attempts to find control of given type within given parent control where given ID matches.

    /// </summary> 
    /// <typeparam name="T">Type of control to find</typeparam> 
    /// <param name="parent">Parent control</param> 
    /// <param name="id">ID of control to find</param> 
    /// <returns>Control if found; null if not.</returns> 
    public static T FindControl<T>(Control parent, string id) where T : Control
    {
        T found = default(T);


        foreach (Control child in parent.Controls)
        {
            if (child is T)
            {
                found = child as T;


                if (found.ID.Equals(id, StringComparison.OrdinalIgnoreCase)) { break; }
            }
            else
            {
                found = FindControl<T>(child, id);


                if (found != null) { break; }
            }
        }


        return found;
    }
}
