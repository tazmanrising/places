using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FrontierModel;
using System.Text.RegularExpressions;

public partial class SnetReports_Results : System.Web.UI.Page
{
    public enum ColumnMapping
    {
        RecordId = 1,        
        Dnis,
        WebDnis,
        CallDateTime,
        Recording,
        TpvAgent,
        SalesAgent,
        DecisionMaker,     
        CompanyName,
        Verified,
        Concern
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        ((Label)Master.FindControl("lblMasterTitle")).Text = "Frontier-SNET Call Search";
        if (!SessionVars.NewSearch)
        {
            Response.Redirect("Default.aspx", true);
        }


        if (!IsPostBack)
        {
            RunSearch();
        }
    }
    private void RunSearch()
    {
        HttpContext.Current.Cache.Remove("result");
        gvSNETReport.DataSource = Data.GetSNETResult();
        gvSNETReport.DataBind();
    }

    protected void gvSNETReport_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            tblSNETMain rec = (tblSNETMain)e.Row.DataItem;

            GridView detail = (GridView)e.Row.FindControl("gvSNETDetail");
            detail.DataSource = rec.tblSNETTns;
            detail.DataBind();

            Calibrus.Recordings.RecordingLocator rl = new Calibrus.Recordings.RecordingLocator(rec.WaveName);

            if (rl.RecordingUrl != null)
            {
                if (rl.Archived == true)
                {
                    e.Row.Cells[(int)ColumnMapping.Recording].Text = "ARCHIVED";
                    e.Row.Cells[(int)ColumnMapping.Recording].Font.Italic = true;
                    e.Row.Cells[(int)ColumnMapping.Recording].Font.Size = FontUnit.XSmall;
                }
                else
                {
                    e.Row.Cells[(int)ColumnMapping.Recording].Text =
                        String.Format(@"<a href=../{0}>Listen</a>", rl.RecordingUrl);
                }
            }
            else
            {
                e.Row.Cells[(int)ColumnMapping.Recording].Text = @"N/A";
                e.Row.Cells[(int)ColumnMapping.Recording].Font.Italic = true;
                e.Row.Cells[(int)ColumnMapping.Recording].Font.Size = FontUnit.XSmall;
            }

        }

    }

    protected void gvSNETReport_RowCommand(object sender, GridViewCommandEventArgs e)
    {

        if (e.CommandName == "Expand" || e.CommandName == "Collapse")
        {
            GridViewRow row = (GridViewRow)((Control)e.CommandSource).Parent.Parent;
            Panel pnl = (Panel)row.FindControl("pnlDetails");
            ImageButton expand = (ImageButton)row.FindControl("btnExpand");
            ImageButton collapse = (ImageButton)row.FindControl("btnCollapse");

            if (e.CommandName == "Expand")
            {
                pnl.Visible = true;
                expand.Visible = false;
                collapse.Visible = true;
            }
            else
            {
                pnl.Visible = false;
                collapse.Visible = false;
                expand.Visible = true;
            }
        }
    }

    protected void gvSNETReport_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvSNETReport.PageIndex = e.NewPageIndex;
        gvSNETReport.DataSource = Data.GetSNETResult();
        gvSNETReport.DataBind();
    }
}