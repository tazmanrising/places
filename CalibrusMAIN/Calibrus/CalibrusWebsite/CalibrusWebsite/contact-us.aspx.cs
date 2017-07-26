using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using CalibrusModel;
using System.Configuration;
using System.Data.EntityClient;
using System.Data.Common;
using System.Text;
using Calibrus.Mail;
using Calibrus.ErrorHandler;

public partial class contact_us : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {


        if (PerformValidation())
        {
            //((Panel)this.FindControl("pnlError")).Visible = false;
            //insert values to db
            InsertRecord();
            //send email
            SendContactInfoEmail();

            //Clear out the form
            ClearContactForm();

            //Set up and show a message confirming the email was sent
            ((Panel)this.FindControl("pnlError")).Visible = true;            
            ((Image)this.FindControl("imgMasterError")).ImageUrl = "images/preferences.png";
            ((Image)this.FindControl("imgMasterError")).AlternateText = "Feedback Sent";
            ((Label)this.FindControl("lblErrorText")).Text = "Your feedback has been sent. Thank you for your inquiry.";           

        }
        else
        {
            //if there are errorrs, scroll page back to top to show validation summary
            //((Panel)this.FindControl("pnlError")).Visible = true;  
            ((Image)this.FindControl("imgMasterError")).ImageUrl = "images/error.png";
            ((Image)this.FindControl("imgMasterError")).AlternateText = "Error";
        }

    }

    private void ClearContactForm()
    {
        txtName.Text = "";
        txtTitle.Text = "";
        txtCompany.Text = "";
        txtPhone.Text = "";
        txtEmail.Text = "";
        ddlState.SelectedIndex = 0;
        txtMessage.Text = "";
    }

    #region Validation
    private bool PerformValidation()
    {
        this.Validate();

        if (this.IsValid)
        {
            ((Panel)this.FindControl("pnlError")).Visible = false;
            return true;
        }
        else
        {
            ((Panel)this.FindControl("pnlError")).Visible = true;
            ((BulletedList)this.FindControl("blErrorList")).Items.Clear();

            foreach (IValidator validationControl in this.Validators)
            {
                if (!validationControl.IsValid)
                {
                    ((BulletedList)this.FindControl("blErrorList")).Items.Add(validationControl.ErrorMessage);
                }
            }

            ((Label)this.FindControl("lblErrorText")).Text = "You must correct the following errors before continuing.";
            return false;
        }
    }
    #endregion

    #region Insert Record
    private tblOnlineFeedbackForm InsertRecord()
    {
        tblOnlineFeedbackForm main = null;
        try
        {
            using (CalibrusEntities data = new CalibrusEntities())
            {
                main = new tblOnlineFeedbackForm();

                main.Name = txtName.Text;
                main.Title = txtTitle.Text.Length == 0 ? null : txtTitle.Text;
                main.Company = txtCompany.Text;
                main.Phone = txtPhone.Text.Length == 0 ? null : StripAllNonNumerics(txtPhone.Text);
                main.Email = txtEmail.Text;
                main.StateAbbrev = ddlState.SelectedIndex == 0 ? null : ddlState.SelectedValue;
                main.Message = EncodeStringInput(txtMessage.Text);

                data.AddTotblOnlineFeedbackForms(main);
                data.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            Calibrus.ErrorHandler.Alerting erAlert = new Calibrus.ErrorHandler.Alerting("Calibrus Website Contact Us:InsertRecord()");
            erAlert.SendAlert(ex.Source, ex.Message, Environment.MachineName, Environment.UserName, "");
        }

        return main;

    }
    #endregion



    #region Email Contact Info
    private void SendContactInfoEmail()
    {
        string strMsgBody = string.Empty;
        string smtpServer = string.Empty;
        string EmailRecipients = string.Empty;

        try
        {
            smtpServer = ConfigurationManager.AppSettings["SMTPServer"].ToString();
            EmailRecipients = ConfigurationManager.AppSettings["EmailRecipients"].ToString();

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("The following party has sent an inquiry via the contact form from the wwww.calibrus.com/contact-us.aspx page. ");
            sb.AppendLine(Environment.NewLine);
            //sb.AppendLine(Environment.NewLine);
            sb.AppendLine("Name: " + txtName.Text);
            //sb.AppendLine(Environment.NewLine);
            sb.AppendLine("Title: " + txtTitle.Text);
            //sb.AppendLine(Environment.NewLine);
            sb.AppendLine("Company: " + txtCompany.Text);
            //sb.AppendLine(Environment.NewLine);
            sb.AppendLine("Phone: " + txtPhone.Text);
            //sb.AppendLine(Environment.NewLine);
            sb.AppendLine("Email: " + txtEmail.Text);
            //sb.AppendLine(Environment.NewLine);
            sb.AppendLine("State: " + ddlState.SelectedValue);
            //sb.AppendLine(Environment.NewLine);
            sb.AppendLine("Message: " + txtMessage.Text);

            strMsgBody = sb.ToString();

            SmtpMail mail = new SmtpMail(smtpServer, false);

            mail.AddRecipient(EmailRecipients, RecipientType.To);
            mail.From = "noreply@calibrus.com";
            mail.Subject = "Calibrus Website Contact Us Inquiry";
            mail.Body = strMsgBody;
            mail.SendMessage();
            sb = null;
            mail = null;
        }
        catch (Exception ex)
        {
            Calibrus.ErrorHandler.Alerting erAlert = new Calibrus.ErrorHandler.Alerting("Calibrus Website Contact Us:SendContactInfoEmail()");
            erAlert.SendAlert(ex.Source, ex.Message, Environment.MachineName, Environment.UserName, "");
        }

    }
    #endregion


    #region AppUtilites

    /// <summary>
    /// Strips out all NonNumeric characters from a string
    /// </summary>
    /// <param name="input">alphanumeric string</param>
    /// <returns>numbers</returns>
    public static string StripAllNonNumerics(string input)
    {
        if (!string.IsNullOrEmpty(input))
        {
            input = Regex.Replace(input, @"[^\d]", "");// strip all non-numeric chars
            return input;
        }
        return string.Empty;
    }

    /// <summary>
    /// Encodes String Input but allows bold and italic tags
    /// </summary>
    /// <param name="input">any string with bold and italic tags</param>
    /// <returns>encoded string</returns>
    public static string EncodeStringInput(string input)
    {

        // Encode the string input
        StringBuilder sb = new StringBuilder(
                                HttpUtility.HtmlEncode(input));
        // Selectively allow <b> and <i>
        sb.Replace("&lt;b&gt;", "<b>");
        sb.Replace("&lt;/b&gt;", "");
        sb.Replace("&lt;i&gt;", "<i>");
        sb.Replace("&lt;/i&gt;", "");


        return sb.ToString();
    }
    #endregion
}