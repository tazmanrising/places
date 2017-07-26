using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FrontierModel;
using System.Text.RegularExpressions;

public partial class E911_Results : System.Web.UI.Page
{
    public enum ColumnMapping
    {
        SubscriberId = 1,
        Name,
        Signature,
        BirthYear,
        TpvAgent,
        TnFormatted,
        Email,
        GeneralAction,
        GeneralDate,
        E911Action,
        E911Date,
        IsData,
        IsVoip,
        User

    }

    public enum ChildColumnMapping
    {
        CallDateTime,
        WavName,
        Disposition,
        TotalTime

    }
    protected void Page_Load(object sender, EventArgs e)
    {

        ((Label)Master.FindControl("lblMasterTitle")).Text = "Frontier E911 Call Search";
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
        gvE911Report.DataSource = Data.GetE911Result();
        gvE911Report.DataBind();
    }

    protected void gvE911Report_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            tblE911LoadFile rec = (tblE911LoadFile)e.Row.DataItem;

            GridView detail = (GridView)e.Row.FindControl("gvE911Detail");
            detail.DataSource = rec.tblE911Main;
            detail.DataBind();        
            
        }

    }
    protected void gvE911Detail_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            tblE911Main rec = (tblE911Main)e.Row.DataItem;

            
            Calibrus.Recordings.RecordingLocator rl = new Calibrus.Recordings.RecordingLocator(rec.WavName);

            if (rl.RecordingUrl != null)
            {
                if (rl.Archived == true)
                {

                    e.Row.Cells[(int)ChildColumnMapping.WavName].Text = "ARCHIVED";
                    e.Row.Cells[(int)ChildColumnMapping.WavName].Font.Italic = true;
                    e.Row.Cells[(int)ChildColumnMapping.WavName].Font.Size = FontUnit.XSmall;
                }
                else
                {
                    e.Row.Cells[(int)ChildColumnMapping.WavName].Text =
                        String.Format(@"<a href=../{0}>Listen</a>", rl.RecordingUrl);
                }
            }
            else
            {
                e.Row.Cells[(int)ChildColumnMapping.WavName].Text = @"N/A";
                e.Row.Cells[(int)ChildColumnMapping.WavName].Font.Italic = true;
                e.Row.Cells[(int)ChildColumnMapping.WavName].Font.Size = FontUnit.XSmall;
            }


        }
    }


    protected void gvE911Report_RowCommand(object sender, GridViewCommandEventArgs e)
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

    protected void gvE911Report_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvE911Report.PageIndex = e.NewPageIndex;
        gvE911Report.DataSource = Data.GetE911Result();
        gvE911Report.DataBind();
    }
}