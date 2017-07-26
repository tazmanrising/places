using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Configuration;
using Calibrus.ErrorHandler;
using Calibrus.Mail;

namespace ConstellationDTDAlertSend
{
    public class AlertSend
    {
        #region Main
        public static void Main(string[] args)
        {
            List<AlertRecord> AlertsToSend = new List<AlertRecord>();

            try
            {


                if (args.Length > 0)
                {
                    int AlertDTDId = 0;

                    if (int.TryParse(args[0], out AlertDTDId))
                    {
                        AlertsToSend = GetSingleAlert(AlertDTDId);//if an AlertDTD.ID arg is passed in via a trigger in the db or command line, we only want to send one alert
                    }
                    else
                    {
                        throw (new ApplicationException("Invalid Parameter"));
                    }
                }
                else
                {
                    AlertsToSend = GetAlerts(); //otherwise this will run by grabbing all alerts that have a SentFlag of 0
                }

                if (AlertsToSend.Count() > 0)
                {

                    foreach (AlertRecord alert in AlertsToSend)
                    {
                        bool SendEmailError = false;
                        try
                        {
                            //Build the Template and get ToList and CcList distro if null from necessary sproc
                            EmailValues emailValue = GetAlertValues(alert.AlertTypeId, alert.AlertType.Trim(), alert.Template, alert.EnrollmentId, alert.MainId, alert.UserId);

                            alert.Template = emailValue.EmailBody;//populate the template with the EmailBody 

                            //append values to alert.ToList                            
                            alert.ToList = IsValueNull(emailValue.ToDistro) ? alert.ToList : string.Format("{0};{1}", alert.ToList, emailValue.ToDistro);
                            //Send Email
                            SendEmail(out SendEmailError, alert.Subject, alert.Template, alert.ToList, alert.CCList);

                            if (!SendEmailError)
                            {
                                //Update Record as successfully
                                UpdateAlertDTD(alert.Id, "1");
                            }
                            else
                            {
                                //Update Record as failed Due to an issue with sending the alert
                                UpdateAlertDTD(alert.Id, "9");
                            
                            }

                        }
                        catch (Exception ex)
                        {
                            LogError(ex, alert.Id);

                            //Update Record as failed
                            UpdateAlertDTD(alert.Id, "9");

                            continue;//don't exit the program keep sending alerts that we are able to
                        }

                    }
                }



            }
            catch (Exception ex)
            {
                SendErrorMessage(ex);
            }
        }
        #endregion Main

        #region Data

        #region Get Data

        /// <summary>
        /// Gets List of alerts that need to be sent via a passed in AlertDTDId
        /// </summary>
        /// <param name="id">int AlertDTD.ID</param>
        /// <returns>List<AlertRecord> of a single AlertRecord</returns>
        private static List<AlertRecord> GetSingleAlert(int id)
        {
            List<AlertRecord> alerts = new List<AlertRecord>();

            using (ConstellationEntities entities = new ConstellationEntities())
            {
                var query = (from a in entities.AlertDTDs
                             join at in entities.AlertTypeDTDs on a.AlertTypeId equals at.Id
                             where a.Id == id
                             //&& (a.EnrollmentId != null && a.UserId != null && a.MainId != null) //must have at least one of these for an alert, however this is not an explicit check for which specific id is required for the AlertTypeID, it is assumed that the alert was inserted with the correct expected value Id for the AlertTypeId
                             select new
                             {
                                 Id = a.Id,
                                 AlertDateTime = a.AlertDateTime,
                                 AlertTypeId = a.AlertTypeId,
                                 EnrollmentId = a.EnrollmentId,
                                 MainId = a.MainId,
                                 UserId = a.UserId,
                                 AlertType = at.Type,
                                 Template = at.Template,
                                 Subject = at.Subject,
                                 ToList = at.ToList,
                                 CCList = at.CCList
                             }).ToList();

                foreach (var item in query)
                {
                    if (item.EnrollmentId != null || item.MainId != null || item.UserId != null)
                    {

                        AlertRecord alert = new AlertRecord(item.Id, item.AlertDateTime, item.AlertTypeId, item.EnrollmentId, item.MainId, item.UserId,
                                                             item.AlertType, item.Template, item.Subject, item.ToList, item.CCList);
                        alerts.Add(alert);
                    }
                    else
                    {
                        //Update Record as failed
                        UpdateAlertDTD(item.Id, "9");
                    }
                }


            }


            return alerts;
        }

        /// <summary>
        /// Gets list of all alerts that need to be sent
        /// </summary>
        /// <returns>List<AlertRecord></returns>
        private static List<AlertRecord> GetAlerts()
        {
            List<AlertRecord> alerts = new List<AlertRecord>();

            using (ConstellationEntities entities = new ConstellationEntities())
            {
                var query = (from a in entities.AlertDTDs
                             join at in entities.AlertTypeDTDs on a.AlertTypeId equals at.Id
                             where a.SentFlag == "0"
                             //&& (a.EnrollmentId != null && a.UserId != null && a.MainId != null) //must have at least one of these for an alert, however this is not an explicit check for which specific id is required for the AlertTypeID, it is assumed that the alert was inserted with the correct expected value Id for the AlertTypeId
                             select new
                             {
                                 Id = a.Id,
                                 AlertDateTime = a.AlertDateTime,
                                 AlertTypeId = a.AlertTypeId,
                                 EnrollmentId = a.EnrollmentId,
                                 MainId = a.MainId,
                                 UserId = a.UserId,
                                 AlertType = at.Type,
                                 Template = at.Template,
                                 Subject = at.Subject,
                                 ToList = at.ToList,
                                 CCList = at.CCList
                             }).ToList();

                foreach (var item in query)
                {
                    if (item.EnrollmentId != null || item.MainId != null || item.UserId != null)
                    {

                        AlertRecord alert = new AlertRecord(item.Id, item.AlertDateTime, item.AlertTypeId, item.EnrollmentId, item.MainId, item.UserId,
                                                             item.AlertType, item.Template, item.Subject, item.ToList, item.CCList);
                        alerts.Add(alert);
                    }
                    else
                    {
                        //Update Record as failed
                        UpdateAlertDTD(item.Id, "9");
                    }
                }


            }


            return alerts;
        }


        /// <summary>
        /// Gets Alert Values for the Alert Record object via stored procedures
        /// </summary>
        /// <param name="alertType"></param>
        /// <param name="enrollmentId"></param>
        /// <param name="mainId"></param>
        /// <param name="userId"></param>
        /// <returns>EmailValues</returns>
        private static EmailValues GetAlertValues(int? alertTypeId, string alertType, string template, int? enrollmentId, int? mainId, int? userId)
        {
            //holds the edited template after matching the values from the sproc and possible To and CC distro lists if returned from the sproc
            EmailValues values = new EmailValues();//the return type of this method
            List<string> getVendorDistro = new List<string>();//returns VendorSpecific Distribution list for specific alerts
            try
            {
                PossibleValues alertdata = new PossibleValues();//holds the possible values that a specific sproc will pull that we need to use for the template
                //get the correct values for the template and ToList or CClist if applicable to a specific vendor or distro based on the sproc results
                switch (alertTypeId)
                {

                    case 1: //"API ConfirmCustomerSignUpByType":
                        spDTDAPIConfirmCustomerSignUpByTypeAlert_Result APIConfirmCustomerSignUpByType_Result = GetAPIConfirmCustomerSignUpByType(mainId);
                        alertdata.MainId = APIConfirmCustomerSignUpByType_Result.MainId;
                        alertdata.CallDateTime = APIConfirmCustomerSignUpByType_Result.CallDateTime;
                        alertdata.Concern = APIConfirmCustomerSignUpByType_Result.Concern;
                        alertdata.ResponseId = APIConfirmCustomerSignUpByType_Result.ResponseId;
                        break;

                    case 2: //"API UpdateTPVStatus() Error":
                        spDTDAPIUpdateTPVStatusErrorAlert_Result APIUpdateTPVStatusError_Result = GetAPIUpdateTPVStatusError(mainId);
                        alertdata.MainId = APIUpdateTPVStatusError_Result.MainId;
                        alertdata.CallDateTime = APIUpdateTPVStatusError_Result.CallDateTime;
                        alertdata.Concern = APIUpdateTPVStatusError_Result.Concern;
                        alertdata.ResponseId = APIUpdateTPVStatusError_Result.ResponseId;
                        break;

                    case 3: //"API UpdateTPVVerificationCode":
                        spDTDAPIUpdateTPVVerificationCodeAlert_Result APIUpdateTPVVerificationCode_Result = GetAPIUpdateTPVVerificationCode(mainId);
                        alertdata.MainId = APIUpdateTPVVerificationCode_Result.MainId;
                        alertdata.CallDateTime = APIUpdateTPVVerificationCode_Result.CallDateTime;
                        alertdata.Concern = APIUpdateTPVVerificationCode_Result.Concern;
                        alertdata.ResponseId = APIUpdateTPVVerificationCode_Result.ResponseId;
                        break;

                    case 4: //"Apt-Unit Number Exceeds Limit":
                        spDTDAptUnitNumberExceedsLimitAlert_Result AptUnitNumberExceedsLimitAlert_Result = GetAptUnitNumberExceedsLimit(mainId);
                        alertdata.MainId = AptUnitNumberExceedsLimitAlert_Result.MainId;
                        alertdata.CallDateTime = AptUnitNumberExceedsLimitAlert_Result.CallDateTime;
                        alertdata.VendorNumber = AptUnitNumberExceedsLimitAlert_Result.VendorNumber;
                        alertdata.AgentId = AptUnitNumberExceedsLimitAlert_Result.AgentId;
                        alertdata.Market = AptUnitNumberExceedsLimitAlert_Result.Market;
                        break;

                    case 5: //"BTN Previously Used":
                        spDTDBTNPreviouslyUsedAlert_Result BTNPreviouslyUsedAlert_Result = GetBTNPreviouslyUsed(enrollmentId);
                        alertdata.CallDateTime = BTNPreviouslyUsedAlert_Result.CallDateTime;
                        alertdata.PreviousCallDateTime = BTNPreviouslyUsedAlert_Result.PreviousCallDateTime;
                        alertdata.BTNUsed = BTNPreviouslyUsedAlert_Result.BTNUsed;
                        alertdata.AgentId = BTNPreviouslyUsedAlert_Result.AgentId;
                        alertdata.SalesAgentName = BTNPreviouslyUsedAlert_Result.SalesAgentName;
                        alertdata.VendorId = BTNPreviouslyUsedAlert_Result.VendorId;
                        alertdata.PreviousCustomerName = BTNPreviouslyUsedAlert_Result.PreviousCustomerName;
                        alertdata.PreviousAddress = BTNPreviouslyUsedAlert_Result.PreviousAddress;
                        alertdata.PreviousVerificationCode = BTNPreviouslyUsedAlert_Result.PreviousVerificationCode;
                        alertdata.PreviousAgentId = BTNPreviouslyUsedAlert_Result.PreviousAgentId;
                        break;

                    case 6: //"Agent Deactivation - BTN":
                        spDTDAgentDeactivationBTNAlert_Result AgentDeactivationBTNAlert_Result = GetAgentDeactivationBTN(enrollmentId);
                        alertdata.CallDateTime = AgentDeactivationBTNAlert_Result.CallDateTime;
                        alertdata.PreviousCallDateTime = AgentDeactivationBTNAlert_Result.PreviousCallDateTime;
                        alertdata.BTNUsed = AgentDeactivationBTNAlert_Result.BTNUsed;
                        alertdata.AgentId = AgentDeactivationBTNAlert_Result.AgentId;
                        alertdata.SalesAgentName = AgentDeactivationBTNAlert_Result.SalesAgentName;
                        alertdata.VendorNumber = AgentDeactivationBTNAlert_Result.VendorNumber;
                        alertdata.VendorId = AgentDeactivationBTNAlert_Result.VendorId.ToString();
                        alertdata.PreviousCustomerName = AgentDeactivationBTNAlert_Result.PreviousCustomerName;
                        alertdata.PreviousAddress = AgentDeactivationBTNAlert_Result.PreviousAddress;
                        alertdata.PreviousVerificationCode = AgentDeactivationBTNAlert_Result.PreviousVerificationCode;
                        alertdata.PreviousAgentId = AgentDeactivationBTNAlert_Result.PreviousAgentId;
                        alertdata.PreviousUDCAccountNumber = AgentDeactivationBTNAlert_Result.PreviousUDCAccountNumber;
                        getVendorDistro = GetAlertDTDDistroListByVendor(alertTypeId, AgentDeactivationBTNAlert_Result.VendorId);
                        values.ToDistro = string.Join(";", getVendorDistro);
                        break;

                    case 7: //"Agent Deactivation - 15 Days Inactivity":
                        spDTDAgentDeactivation15DaysInactivityAlert_Result AgentDeactivation15DaysInactivityAlert_Result = GetAgentDeactivation15DaysInactivity(userId);
                        alertdata.MainId = AgentDeactivation15DaysInactivityAlert_Result.MainId;
                        alertdata.CallDateTime = AgentDeactivation15DaysInactivityAlert_Result.CallDateTime;
                        alertdata.VendorNumber = AgentDeactivation15DaysInactivityAlert_Result.VendorNumber;
                        alertdata.VendorId = AgentDeactivation15DaysInactivityAlert_Result.VendorId.ToString();
                        alertdata.AgentId = AgentDeactivation15DaysInactivityAlert_Result.AgentId;
                        getVendorDistro = GetAlertDTDDistroListByVendor(alertTypeId, AgentDeactivation15DaysInactivityAlert_Result.VendorId);
                        values.ToDistro = string.Join(";", getVendorDistro);
                        break;

                    case 8: //"Agent with 6 or more sales in one day":
                        spDTDAgentWith6OrMoreSalesInOneDayAlert_Result AgentWith6OrMoreSalesInOneDayAlert_Result = GetAgentWith6OrMoreSalesInOneDay(mainId);
                        alertdata.MainId = AgentWith6OrMoreSalesInOneDayAlert_Result.MainId;
                        alertdata.CallDateTime = AgentWith6OrMoreSalesInOneDayAlert_Result.CallDateTime;
                        alertdata.VendorNumber = AgentWith6OrMoreSalesInOneDayAlert_Result.VendorNumber;
                        alertdata.AgentId = AgentWith6OrMoreSalesInOneDayAlert_Result.AgentId;
                        alertdata.FirstName = AgentWith6OrMoreSalesInOneDayAlert_Result.FirstName;
                        alertdata.LastName = AgentWith6OrMoreSalesInOneDayAlert_Result.LastName;
                        break;

                    case 9: //"Agent with 2 or more sales in one day - Tx Only":
                        spDTDAgentWith2OrMoreSalesInOneDayTxOnlyAlert_Result AgentWith2OrMoreSalesInOneDayTxOnlyAlert_Result = GetAgentWith2OrMoreSalesInOneDayTxOnly(mainId);
                        alertdata.MainId = AgentWith2OrMoreSalesInOneDayTxOnlyAlert_Result.MainId;
                        alertdata.CallDateTime = AgentWith2OrMoreSalesInOneDayTxOnlyAlert_Result.CallDateTime;
                        alertdata.VendorNumber = AgentWith2OrMoreSalesInOneDayTxOnlyAlert_Result.VendorNumber;
                        alertdata.AgentId = AgentWith2OrMoreSalesInOneDayTxOnlyAlert_Result.AgentId;
                        alertdata.FirstName = AgentWith2OrMoreSalesInOneDayTxOnlyAlert_Result.FirstName;
                        alertdata.LastName = AgentWith2OrMoreSalesInOneDayTxOnlyAlert_Result.LastName;
                        break;

                    case 10: //"Agent Call Back Number":
                        spDTDAgentCallBackNumberAlert_Result AgentCallBackNumberAlert_Result = GetAgentCallBackNumber(enrollmentId);
                        alertdata.CallDateTime = AgentCallBackNumberAlert_Result.CallDateTime;
                        alertdata.VendorId = AgentCallBackNumberAlert_Result.VendorId;
                        alertdata.CallBackNumber = AgentCallBackNumberAlert_Result.CallBackNumber;
                        alertdata.TlpAgent = AgentCallBackNumberAlert_Result.TlpAgent;
                        break;

                    case 11: //"Call Back Number Already Exists On Good Sale":
                        spDTDCallBackNumberAlreadyExistsOnGoodSaleAlert_Result CallBackNumberAlreadyExistsOnGoodSaleAlert_Result = GetCallBackNumberAlreadyExistsOnGoodSale(enrollmentId);
                        alertdata.CallDateTime = CallBackNumberAlreadyExistsOnGoodSaleAlert_Result.CallDateTime;
                        alertdata.VendorId = CallBackNumberAlreadyExistsOnGoodSaleAlert_Result.VendorId;
                        alertdata.CallBackNumber = CallBackNumberAlreadyExistsOnGoodSaleAlert_Result.CallBackNumber;
                        alertdata.TlpAgent = CallBackNumberAlreadyExistsOnGoodSaleAlert_Result.TlpAgent;
                        break;

                    case 12: //"Lot or Trailer Alert":
                        spDTDLotOrTrailerAlert_Result LotOrTrailerAlert_Result = GetLotOrTrailer(mainId);
                        alertdata.CallDateTime = LotOrTrailerAlert_Result.CallDateTime;
                        alertdata.VendorId = LotOrTrailerAlert_Result.VendorId;
                        alertdata.TlpAgent = LotOrTrailerAlert_Result.TlpAgent;
                        alertdata.BillingAddress1 = LotOrTrailerAlert_Result.BillingAddress1;
                        alertdata.BillingAddress2 = LotOrTrailerAlert_Result.BillingAddress2;
                        alertdata.BillingCity = LotOrTrailerAlert_Result.City;
                        alertdata.BillingState = LotOrTrailerAlert_Result.State;
                        alertdata.BillingZip = LotOrTrailerAlert_Result.Zip;
                        break;

                    case 13: //"No Sale":
                        spDTDNoSaleAlert_Result NoSaleAlert_Result = GetNoSale(mainId);
                        alertdata.CallDateTime = NoSaleAlert_Result.CallDateTime;
                        alertdata.VendorNumber = NoSaleAlert_Result.VendorNumber;
                        alertdata.VendorId = NoSaleAlert_Result.VendorId.ToString();
                        alertdata.ResponseId = NoSaleAlert_Result.ResponseId;
                        alertdata.AgentId = NoSaleAlert_Result.AgentId;
                        alertdata.MainId = NoSaleAlert_Result.MainId;
                        alertdata.Concern = NoSaleAlert_Result.Concern;
                        getVendorDistro = GetAlertDTDDistroListByVendor(alertTypeId, NoSaleAlert_Result.VendorId);
                        values.ToDistro = string.Join(";", getVendorDistro);
            
                        break;

                    case 14: //"POS ID Errors":
                        spDTDPOSIDErrorsAlert_Result POSIDErrorsAlert_Result = GetPOSIDErrors(mainId);
                        alertdata.CallDateTime = POSIDErrorsAlert_Result.CallDateTime;
                        alertdata.ResponseId = POSIDErrorsAlert_Result.ResponseId;
                        alertdata.MainId = POSIDErrorsAlert_Result.MainId;
                        alertdata.AgentId = POSIDErrorsAlert_Result.AgentId;
                        alertdata.Status = POSIDErrorsAlert_Result.Status; ;
                        break;

                    case 15: //"Agent Did Not Leave Premises":
                        spDTDAgentDidNotLeavePremisesAlert_Result AgentDidNotLeavePremisesAlert_Result = GetAgentDidNotLeavePremises(mainId);
                        alertdata.CallDateTime = AgentDidNotLeavePremisesAlert_Result.CallDateTime;
                        alertdata.VendorNumber = AgentDidNotLeavePremisesAlert_Result.VendorNumber;
                        alertdata.AgentId = AgentDidNotLeavePremisesAlert_Result.AgentId;
                        alertdata.ResponseId = AgentDidNotLeavePremisesAlert_Result.ResponseId;
                        alertdata.MainId = AgentDidNotLeavePremisesAlert_Result.MainId;
                        break;

                    case 16: //"RecordLocatorPreviouslyUsed":
                        //NEED TO BUILD ALERT DATA                      
                        values.ToDistro = "";
                        break;
                }

                values.EmailBody = GetEmailBody(alertdata, template);
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw ex;
            }
            return values;
        }

        #region Stored Procedures

        #region GetAlertDTDDistroList
        private static List<string> GetAlertDTDDistroListByVendor(int? alertTypeDTDId, int? vendorId)
        {
            List<string> result = null;
            using (ConstellationEntities entities = new ConstellationEntities())
            {
                result = entities.spGetAlertDTDDistroListByVendor(alertTypeDTDId: alertTypeDTDId, vendorId: vendorId).ToList();
            }
            return result;
        }
        #endregion GetAlertDTDDistroList

        #region AlertDTDType
        private static spDTDAPIConfirmCustomerSignUpByTypeAlert_Result GetAPIConfirmCustomerSignUpByType(int? mainId)
        {
            spDTDAPIConfirmCustomerSignUpByTypeAlert_Result result = null;
            using (ConstellationEntities entities = new ConstellationEntities())
            {
                result = entities.spDTDAPIConfirmCustomerSignUpByTypeAlert(mainid: mainId).FirstOrDefault();
            }
            return result;
        }
        private static spDTDAPIUpdateTPVStatusErrorAlert_Result GetAPIUpdateTPVStatusError(int? mainId)
        {
            spDTDAPIUpdateTPVStatusErrorAlert_Result result = null;
            using (ConstellationEntities entities = new ConstellationEntities())
            {
                result = entities.spDTDAPIUpdateTPVStatusErrorAlert(mainid: mainId).FirstOrDefault();
            }
            return result;
        }
        private static spDTDAPIUpdateTPVVerificationCodeAlert_Result GetAPIUpdateTPVVerificationCode(int? mainId)
        {
            spDTDAPIUpdateTPVVerificationCodeAlert_Result result = null;
            using (ConstellationEntities entities = new ConstellationEntities())
            {
                result = entities.spDTDAPIUpdateTPVVerificationCodeAlert(mainid: mainId).FirstOrDefault();
            }
            return result;
        }
        private static spDTDAptUnitNumberExceedsLimitAlert_Result GetAptUnitNumberExceedsLimit(int? mainId)
        {
            spDTDAptUnitNumberExceedsLimitAlert_Result result = null;
            using (ConstellationEntities entities = new ConstellationEntities())
            {
                result = entities.spDTDAptUnitNumberExceedsLimitAlert(mainid: mainId).FirstOrDefault();
            }
            return result;
        }
        private static spDTDBTNPreviouslyUsedAlert_Result GetBTNPreviouslyUsed(int? enrollmentId)
        {
            spDTDBTNPreviouslyUsedAlert_Result result = null;
            using (ConstellationEntities entities = new ConstellationEntities())
            {
                result = entities.spDTDBTNPreviouslyUsedAlert(enrollmentId: enrollmentId).FirstOrDefault();
            }
            return result;
        }
        private static spDTDAgentDeactivationBTNAlert_Result GetAgentDeactivationBTN(int? enrollmentId)
        {
            spDTDAgentDeactivationBTNAlert_Result result = null;
            using (ConstellationEntities entities = new ConstellationEntities())
            {
                result = entities.spDTDAgentDeactivationBTNAlert(enrollmentId: enrollmentId).FirstOrDefault();
            }
            return result;
        }
        private static spDTDAgentDeactivation15DaysInactivityAlert_Result GetAgentDeactivation15DaysInactivity(int? userId)
        {
            spDTDAgentDeactivation15DaysInactivityAlert_Result result = null;
            using (ConstellationEntities entities = new ConstellationEntities())
            {
                result = entities.spDTDAgentDeactivation15DaysInactivityAlert(userId: userId).FirstOrDefault();
            }
            return result;
        }
        private static spDTDAgentWith6OrMoreSalesInOneDayAlert_Result GetAgentWith6OrMoreSalesInOneDay(int? mainId)
        {
            spDTDAgentWith6OrMoreSalesInOneDayAlert_Result result = null;
            using (ConstellationEntities entities = new ConstellationEntities())
            {
                result = entities.spDTDAgentWith6OrMoreSalesInOneDayAlert(mainid: mainId).FirstOrDefault();
            }
            return result;
        }
        private static spDTDAgentWith2OrMoreSalesInOneDayTxOnlyAlert_Result GetAgentWith2OrMoreSalesInOneDayTxOnly(int? mainId)
        {
            spDTDAgentWith2OrMoreSalesInOneDayTxOnlyAlert_Result result = null;
            using (ConstellationEntities entities = new ConstellationEntities())
            {
                result = entities.spDTDAgentWith2OrMoreSalesInOneDayTxOnlyAlert(mainid: mainId).FirstOrDefault();
            }
            return result;
        }
        private static spDTDAgentCallBackNumberAlert_Result GetAgentCallBackNumber(int? enrollmentId)
        {
            spDTDAgentCallBackNumberAlert_Result result = null;
            using (ConstellationEntities entities = new ConstellationEntities())
            {
                result = entities.spDTDAgentCallBackNumberAlert(enrollmentId: enrollmentId).FirstOrDefault();
            }
            return result;
        }
        private static spDTDCallBackNumberAlreadyExistsOnGoodSaleAlert_Result GetCallBackNumberAlreadyExistsOnGoodSale(int? enrollmentId)
        {
            spDTDCallBackNumberAlreadyExistsOnGoodSaleAlert_Result result = null;
            using (ConstellationEntities entities = new ConstellationEntities())
            {
                result = entities.spDTDCallBackNumberAlreadyExistsOnGoodSaleAlert(enrollmentId: enrollmentId).FirstOrDefault();
            }
            return result;
        }
        private static spDTDLotOrTrailerAlert_Result GetLotOrTrailer(int? mainId)
        {
            spDTDLotOrTrailerAlert_Result result = null;
            using (ConstellationEntities entities = new ConstellationEntities())
            {
                result = entities.spDTDLotOrTrailerAlert(mainId: mainId).FirstOrDefault();
            }
            return result;
        }
        private static spDTDNoSaleAlert_Result GetNoSale(int? mainId)
        {
            spDTDNoSaleAlert_Result result = null;
            using (ConstellationEntities entities = new ConstellationEntities())
            {
                result = entities.spDTDNoSaleAlert(mainid: mainId).FirstOrDefault();
            }
            return result;
        }
        private static spDTDPOSIDErrorsAlert_Result GetPOSIDErrors(int? mainId)
        {
            spDTDPOSIDErrorsAlert_Result result = null;
            using (ConstellationEntities entities = new ConstellationEntities())
            {
                result = entities.spDTDPOSIDErrorsAlert(mainid: mainId).FirstOrDefault();
            }
            return result;
        }
        private static spDTDAgentDidNotLeavePremisesAlert_Result GetAgentDidNotLeavePremises(int? mainId)
        {
            spDTDAgentDidNotLeavePremisesAlert_Result result = null;
            using (ConstellationEntities entities = new ConstellationEntities())
            {
                result = entities.spDTDAgentDidNotLeavePremisesAlert(mainid: mainId).FirstOrDefault();
            }
            return result;
        }
        #endregion AlertDTDType

        #endregion Stored Procedures

        #endregion Get Data

        #region Update Data

        private static void UpdateAlertDTD(int? alertId, string sentFlag)
        {
            try
            {
                AlertDTD alertDTD = null;
                using (ConstellationEntities entities = new ConstellationEntities())
                {
                    alertDTD = (from record in entities.AlertDTDs
                                where record.Id == alertId
                                select record).FirstOrDefault();

                    alertDTD.SentFlag = sentFlag;
                    alertDTD.SentDateTime = DateTime.Now;
                    entities.SaveChanges();

                }

            }
            catch (Exception ex)
            {
                LogError(ex, alertId);
            }
        }
        #endregion Update Data

        #endregion Data

        #region Utilities

        /// <summary>
        /// Builds the Email Body we intend to send based on the Template and PossibleValues passed in
        /// </summary>
        /// <param name="alertdata"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        private static string GetEmailBody(PossibleValues alertdata, string template)
        {
            MatchCollection matches = Regex.Matches(template, @"{{\w+}}");
            foreach (Match match in matches)
            {
                string propertyName = Regex.Replace(match.Value, "({{)|(}})", "");
                System.Reflection.PropertyInfo propertyInfo = alertdata.GetType().GetProperty(propertyName);
                if (propertyInfo != null)
                {
                    template = template.Replace(match.Value, propertyInfo.GetValue(alertdata, null) == null ? "" : propertyInfo.GetValue(alertdata, null).ToString());
                }
            }
            return template;
        }


        /// <summary>
        /// Takes the value passed in and tests to see if it is a NULL type, 
        /// that being empty string, white space or a string type of NULL
        /// </summary>
        /// <param name="value">string</param>
        /// <returns>true or false</returns>
        private static bool IsValueNull(string value)
        {
            bool status = false;
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value) || value.ToUpper() == "NULL")
            {
                status = true;
            }
            return status;
        }


        private static void SendEmail(out bool SendEmailError, string strSubject, string strTemplate, string strToEmail, string strCcEmail)
        {
            //string strMsgBody = string.Empty;
            try
            {
                SendEmailError = false;
                //StringBuilder sb = new StringBuilder();

                //sb.AppendLine("You have completed the Investor Verification process.  Attached is a summary report showing all data  ");
                //sb.AppendLine("entered into the investor verification website and a wav.file of your phone verification recording  ");
                //sb.AppendLine("indicating your verbal verification.   ");
                //sb.AppendLine(Environment.NewLine);
                //sb.AppendLine("If you have any questions please call us at (800) 222-2222.");
                //sb.AppendLine(Environment.NewLine);
                //sb.AppendLine("Sincerely,");
                //sb.AppendLine(Environment.NewLine);
                //sb.AppendLine("The Calibrus Verification Team");
                //strMsgBody = sb.ToString();



                SmtpMail mail = new SmtpMail("TMPWEB1", false);

                mail.IsBodyHtml = true;
                mail.AddRecipient(strToEmail, RecipientType.To);
                if (!IsValueNull(strCcEmail))
                {
                    mail.AddRecipient(strCcEmail, RecipientType.Cc);
                }

                mail.From = "reports1@calibrus.com";

                mail.Subject = strSubject;

                mail.Body = strTemplate;

                mail.SendMessage();

            }
            catch (Exception ex)
            {
                SendEmailError = true;
                SendErrorMessage(ex);
            }

        }
        #endregion Utilities

        #region ErrorHandling

        static void SendErrorMessage(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("ex:{0}, innerEx:{1}", ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);

            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("ConstellationDTDAlertSend");
            alert.SendAlert(ex.Source, sb.ToString(), Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }

        static void SendErrorMessage(Exception ex, int? alertid)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("alertid:{0}, ex:{1}, innerEx:{2}", alertid, ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);

            Calibrus.ErrorHandler.Alerting alert = new Calibrus.ErrorHandler.Alerting("ConstellationDTDAlertSend");
            alert.SendAlert(ex.Source, sb.ToString(), Environment.MachineName, Environment.UserName, Environment.Version.ToString());
        }


        static void LogError(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("ex:{0}, innerEx:{1}", ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);

            Calibrus.ErrorHandler.Logging log = new Calibrus.ErrorHandler.Logging();
            log.LogError("ConstellationDTDAlertSend", Environment.Version.ToString(), Environment.MachineName, Environment.UserName, ex.Source, sb.ToString());
        }

        static void LogError(Exception ex, int? alertId)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("alertid:{0}, ex:{1}, innerEx:{2}", alertId, ex.Message, ex.InnerException == null ? "" : ex.InnerException.Message);

            Calibrus.ErrorHandler.Logging log = new Calibrus.ErrorHandler.Logging();
            log.LogError("ConstellationDTDAlertSend", Environment.Version.ToString(), Environment.MachineName, Environment.UserName, ex.Source, sb.ToString());
        }

        #endregion
    }
}
