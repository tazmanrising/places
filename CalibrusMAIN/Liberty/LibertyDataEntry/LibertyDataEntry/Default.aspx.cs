using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LibertyModel;

public partial class _Default : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        if (PerformValidation())
        {
            ((Panel)this.FindControl("pnlError")).Visible = false;
            
            SessionVars.AdminUser = GetAdminUser();
            Response.Redirect("DataEntry.aspx");
        }
        else
        {
            ((Panel)this.FindControl("pnlError")).Visible = true;
        }

    }

    private bool PerformValidation()
    {
        this.Validate();

        if (this.IsValid)
            return true;
        else
        {
            ((BulletedList)this.FindControl("blErrorList")).Items.Clear();

            foreach (IValidator validationControl in this.Validators)
            {
                validationControl.Validate();

                if (!validationControl.IsValid)
                {
                    ((BulletedList)this.FindControl("blErrorList")).Items.Add(validationControl.ErrorMessage);
                }
            }

            return false;
        }
    }

    protected void cvCheckLogin_ServerValidate(object source, ServerValidateEventArgs args)
    {
        bool valid = false;
        tblUserLogin admin = GetAdminUser();

        if (admin != null)
        {
            valid = admin.Active;
        }

        args.IsValid = valid;
    }

    private tblUserLogin GetAdminUser()
    {
        using (LibertyEntities entities = new LibertyModel.LibertyEntities())
        {
            tblUserLogin admin = entities.tblUserLogins.FirstOrDefault(x => x.UserName.Equals(txtUserName.Text, StringComparison.CurrentCultureIgnoreCase) &&
                x.Password.Equals(txtPassword.Text, StringComparison.CurrentCultureIgnoreCase));
            return admin;
        }
    }
}
