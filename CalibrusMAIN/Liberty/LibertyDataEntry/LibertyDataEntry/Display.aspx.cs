using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Display : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        lblRecordLocator.Text = SessionVars.CurrentAccount.MainId.ToString();
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        Response.Redirect("DataEntry.aspx", true);
    }
}