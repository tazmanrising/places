using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Site : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //need this to resolve <%#ResolveUrl(.. 
        //which is originally <%=ResolveUrl(...
        Page.Header.DataBind();  
    }
}
