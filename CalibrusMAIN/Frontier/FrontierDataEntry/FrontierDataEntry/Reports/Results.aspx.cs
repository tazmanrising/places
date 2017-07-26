using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FrontierModel;
using System.Text.RegularExpressions;

public partial class Reports_Results : System.Web.UI.Page
{
    public enum ColumnMapping
    {
        RecordId = 1,
        CallDateTime,
        Dnis,
        Recording,
        TpvAgent,
        SalesAgent,
        DecisionMaker,
        CustomerName,
        CompanyName,
        Product,
        State,
        Verified,
        Concern
    }

    protected void Page_Load(object sender, EventArgs e)
    {

        ((Label)Master.FindControl("lblMasterTitle")).Text = "Frontier Call Search";
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
        gvReport.DataSource = Data.GetResult();
        gvReport.DataBind();
    }

    protected void gvReport_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            tblMain rec = (tblMain)e.Row.DataItem;

            GridView detail = (GridView)e.Row.FindControl("gvDetail");
            detail.DataSource = rec.tblTns;
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

    protected void gvReport_RowCommand(object sender, GridViewCommandEventArgs e)
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

    protected void gvReport_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvReport.PageIndex = e.NewPageIndex;
        gvReport.DataSource = Data.GetResult();
        gvReport.DataBind();
    }
}