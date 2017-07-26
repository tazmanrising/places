using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Calibrus.Mail;
using System.Configuration;
using System.Text;

namespace ConstellationScriptViewer.Utilities
{
    public static class AppLogic
    {
        public static void EmailScriptChanges(ConstellationScriptViewer.Models.EmailObject emailObj)
        {
            string strMsgBody = string.Empty;
            string smtpServer = string.Empty;
            string EmailRecipientsTO = string.Empty;
            try
            {
                smtpServer = ConfigurationManager.AppSettings["SMTPServer"].ToString();
                EmailRecipientsTO = ConfigurationManager.AppSettings["EmailRecipientsTO"].ToString();

                StringBuilder sb = new StringBuilder();

                sb.AppendLine(string.Format("Client DB: {0}", emailObj.Client));
                sb.AppendLine();
                sb.AppendLine(string.Format("Script Name: {0}", emailObj.Script));
                sb.AppendLine();
                sb.AppendLine(string.Format("Script Id: {0}", emailObj.ScriptId));
                sb.AppendLine();
                sb.AppendLine(string.Format("Script Order: {0}", emailObj.ScriptOrder));
                sb.AppendLine();
                sb.AppendLine(string.Format("Active: {0}", emailObj.Active));
                sb.AppendLine();
                sb.AppendLine(string.Format("YesNo: {0}", emailObj.YesNo));
                sb.AppendLine();
                sb.AppendLine(string.Format("Verbiage English: {0}", emailObj.Verbiage));
                sb.AppendLine();
                sb.AppendLine(string.Format("Verbiage Spanish: {0}", emailObj.VerbiageSpanish));
                sb.AppendLine();
                sb.AppendLine(string.Format("No Verbiage: {0}", emailObj.NoVerbiage));
                sb.AppendLine();
                sb.AppendLine(string.Format("No Verbiage Spanish: {0}", emailObj.NoVerbiageSpanish));
                sb.AppendLine();
                sb.AppendLine(string.Format("Condition: {0}", emailObj.Condition));
                sb.AppendLine();
                sb.AppendLine(string.Format("No Concern Code: {0}", emailObj.NoConcernCode));
                sb.AppendLine();
                sb.AppendLine(string.Format("Customer Notes: {0}", emailObj.Notes));
                strMsgBody = sb.ToString();

                SmtpMail mail = new SmtpMail(smtpServer, false);

                mail.AddRecipient(EmailRecipientsTO, RecipientType.To);
                if (!string.IsNullOrEmpty(emailObj.CCDistro))
                {
                    mail.AddRecipient(emailObj.CCDistro, RecipientType.Cc);
                }
                mail.From = "noreply@calibrus.com";
                mail.Subject = string.Format("{0} Script Change Request for {1} ", emailObj.Client, emailObj.Script);
                mail.Body = strMsgBody;
                mail.SendMessage();
                sb = null;
                mail = null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}