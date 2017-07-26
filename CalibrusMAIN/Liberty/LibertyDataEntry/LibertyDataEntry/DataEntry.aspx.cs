using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Text;
using LibertyModel;

public partial class DataEntry : System.Web.UI.Page
{

    public enum ColumnMapping
    {

        Btn,
        AccountNumber,
        ServiceAddress1,
        ServiceCity,
        ServiceState,
        ServiceZip,
        OrderDetailFormRecordNumber

    }
    protected void Page_Load(object sender, EventArgs e)
    {
        //mpePopUp.Show(); //delete after testing


        //kick user back to root if they are not authorized to log in
        if (SessionVars.AdminUser == null)
        {
            Response.Redirect("~");
        }



        if (!IsPostBack)
        {
            //Set current date to make sure they cannot enter in a date before today.
            //CompareRateEffectiveDateTodayValidator.ValueToCompare = DateTime.Now.ToShortDateString();

            initForm();
        }

        //Grid View show if data exists in List<OrderDetailRecordFormList>() object
        //ShowGridView();

    }

    #region Form Initializations

    private void initForm()
    {
        //clean up session vars
        //SessionVars.AdminUser = null;//do not uncomment this one, this is necessary to allow users to use this page
        //SessionVars.Vendor = null;
        SessionVars.Vendor = null;
        SessionVars.Office = null;
        SessionVars.SalesChannel = null;
        SessionVars.AccountEditMode = false;
        SessionVars.OrderDetailFormRecordNumber = 0;
        SessionVars.MainRecord = null;
        SessionVars.OrderDetailRecord = null;
        SessionVars.MainFormRecord = null;
        SessionVars.OrderDetailFormRecord = null;
        SessionVars.OrderDetailFormRecordList = null;
        SessionVars.CurrentAccount = null;

        SessionVars.Office = GetOffice(SessionVars.AdminUser.OfficeId);
        //kick user if no Active Offices are Returned
        if (SessionVars.Office == null)
        {
            Response.Redirect("~");
        }

        SessionVars.Vendor = GetVendor(SessionVars.AdminUser.VendorId);
        //kick user if no Active Vendors are Returned
        if (SessionVars.Vendor == null)
        {
            Response.Redirect("~");
        }

        SessionVars.SalesChannel = GetSalesChannel(SessionVars.Vendor.SalesChannelId);
        //kick user if no Active Sales Channel are Returned
        if (SessionVars.SalesChannel == null)
        {
            Response.Redirect("~");
        }

        //Set ddlSalesChannelID and make it readonly
        //ddlSalesChannelId.Items.Add(new ListItem(SessionVars.SalesChannel.Name, SessionVars.SalesChannel.SalesChannelId.ToString()));
        ddlSalesChannelId.Items.Add(new ListItem(SessionVars.Office.OfficeName, SessionVars.Vendor.SalesChannelId.ToString()));
        ddlSalesChannelId.SelectedItem.Value = SessionVars.SalesChannel.SalesChannelId.ToString();
        ddlSalesChannelId.Enabled = false;

        ResetMarketDropDowns();

        ResetEffectiveRateValidation();

        //turn off Submit button
        btnSubmit.Visible = false;

    }

    private void initFormFromPhoneSearch(Main mainrecord)
    {
        //Get first OrderDetail Record associated with the Main, we are only populating the data
        //from the first OD record and not populating any modalform or gridview objects
        OrderDetail orderdetailrecord = mainrecord.OrderDetails.FirstOrDefault();


        ddlMarketState.ClearSelection();
        ddlMarketState.Items.FindByValue(mainrecord.MarketStateId.ToString()).Selected = true;
        ddlMarketState_SelectedIndexChanged(this, EventArgs.Empty);


        //TODO: Add IF statements for Nullable fields coming back if it was a gas only.
        //Electric
        ddlMarketUtility.ClearSelection();
        ddlMarketUtility.Items.FindByValue(mainrecord.MarketUtilityId.ToString()).Selected = true;
        ddlMarketUtility_SelectedIndexChanged(this, EventArgs.Empty);

        ddlMarketProduct.ClearSelection();
        ddlMarketProduct.Items.FindByValue(mainrecord.MarketProductId.ToString()).Selected = true;
        ddlMarketProduct_SelectedIndexChanged(this, EventArgs.Empty);

        if (!string.IsNullOrEmpty(mainrecord.DeliveryZoneId.ToString()))
        {
            ddlDeliveryZone.Items.FindByValue(mainrecord.DeliveryZoneId.ToString()).Selected = true;
        }

        txtAuthorizationFirstName.Text = mainrecord.AuthorizationFirstName;
        txtAuthorizationLasttName.Text = mainrecord.AuthorizationLastName;
        txtEmail.Text = mainrecord.Email;
        txtBusinessTaxId.Text = !string.IsNullOrEmpty(mainrecord.BusinessTaxId) ? mainrecord.BusinessTaxId : string.Empty;
        txtBusinessName.Text = !string.IsNullOrEmpty(mainrecord.BusinessName) ? mainrecord.BusinessName : string.Empty;

        ddlSohoAccount.ClearSelection();
        ddlSohoAccount.Items.FindByValue(!string.IsNullOrEmpty(mainrecord.SOHOAccount) ? mainrecord.SOHOAccount : string.Empty).Selected = true;

        //Electric
        ddlContractTerm.ClearSelection();
        ddlContractTerm.Items.FindByValue(mainrecord.ContractTermId.ToString()).Selected = true;

        //txtNumOfAccounts.Text = mainrecord.NumberOfAccounts;

        txtEnergyGRT.Text = !string.IsNullOrEmpty(mainrecord.EnergyGRT) ? mainrecord.EnergyGRT : string.Empty;
        txtContractId.Text = !string.IsNullOrEmpty(mainrecord.ContractID) ? mainrecord.ContractID : string.Empty;
        txtEFLVersionCode.Text = !string.IsNullOrEmpty(mainrecord.EFLVersionCode) ? mainrecord.EFLVersionCode : string.Empty;

        ddlSwitchOrMoveIn.ClearSelection();
        ddlSwitchOrMoveIn.Items.FindByValue(!string.IsNullOrEmpty(mainrecord.SwitchOrMoveIn) ? mainrecord.SwitchOrMoveIn : string.Empty).Selected = true;

        ddlMaterialPreference.ClearSelection();
        ddlMaterialPreference.Items.FindByValue(!string.IsNullOrEmpty(mainrecord.MaterialPreference) ? mainrecord.MaterialPreference : string.Empty).Selected = true;

        txtRateExpirationDate.Text = !string.IsNullOrEmpty(mainrecord.EstDateExpiration) ? mainrecord.EstDateExpiration : string.Empty;

        txtRate.Text = !string.IsNullOrEmpty(mainrecord.Rate) ? mainrecord.Rate : string.Empty;
        txtRateEffectiveDate.Text = mainrecord.RateEffectiveDate;
        txtSubTermMonth1Start.Text = !string.IsNullOrEmpty(mainrecord.SubTermMonth1Start) ? mainrecord.SubTermMonth1Start : string.Empty;
        txtSubTermMonth1End.Text = !string.IsNullOrEmpty(mainrecord.SubTermMonth1End) ? mainrecord.SubTermMonth1End : string.Empty;
        txtSubTermMonth2Start.Text = !string.IsNullOrEmpty(mainrecord.SubTermMonth2Start) ? mainrecord.SubTermMonth2Start : string.Empty;
        txtSubTermMonth2End.Text = !string.IsNullOrEmpty(mainrecord.SubTermMonth2End) ? mainrecord.SubTermMonth2End : string.Empty;
        txtSubTermMonth3Start.Text = !string.IsNullOrEmpty(mainrecord.SubTermMonth3Start) ? mainrecord.SubTermMonth3Start : string.Empty;
        txtSubTermMonth3End.Text = !string.IsNullOrEmpty(mainrecord.SubTermMonth3End) ? mainrecord.SubTermMonth3End : string.Empty;
        txtSubTermMonth4Start.Text = !string.IsNullOrEmpty(mainrecord.SubTermMonth4Start) ? mainrecord.SubTermMonth4Start : string.Empty;
        txtSubTermMonth4End.Text = !string.IsNullOrEmpty(mainrecord.SubTermMonth4End) ? mainrecord.SubTermMonth4End : string.Empty;


        txtAccountNumber.Text = orderdetailrecord.AccountNumber;
        txtNameKey.Text = !string.IsNullOrEmpty(orderdetailrecord.NameKey) ? orderdetailrecord.NameKey : string.Empty;
        txtServiceNumber.Text = !string.IsNullOrEmpty(orderdetailrecord.ServiceNumber) ? orderdetailrecord.ServiceNumber : string.Empty;
        txtSubTermMonth1Rate.Text = !string.IsNullOrEmpty(orderdetailrecord.SubTermRate1) ? orderdetailrecord.SubTermRate1 : string.Empty;
        txtSubTermMonth2Rate.Text = !string.IsNullOrEmpty(orderdetailrecord.SubTermRate2) ? orderdetailrecord.SubTermRate2 : string.Empty;
        txtSubTermMonth3Rate.Text = !string.IsNullOrEmpty(orderdetailrecord.SubTermRate3) ? orderdetailrecord.SubTermRate3 : string.Empty;
        txtSubTermMonth4Rate.Text = !string.IsNullOrEmpty(orderdetailrecord.SubTermRate4) ? orderdetailrecord.SubTermRate4 : string.Empty;

        txtServiceAddress1.Text = orderdetailrecord.ServiceAddress;
        txtServiceAddress2.Text = !string.IsNullOrEmpty(orderdetailrecord.ServiceAddress2) ? orderdetailrecord.ServiceAddress2 : string.Empty;

        txtServiceZip.Text = orderdetailrecord.ServiceZip;
        serviceZipSearch_Click(this, EventArgs.Empty);

        ddlServiceCity.ClearSelection();
        ddlServiceCity.Items.FindByText(orderdetailrecord.ServiceCity).Selected = true;
        ddlServiceState.ClearSelection();
        ddlServiceState.Items.FindByText(orderdetailrecord.ServiceState).Selected = true;

        txtBillingAddress1.Text = orderdetailrecord.BillingAddress;
        txtBillingAddress2.Text = !string.IsNullOrEmpty(orderdetailrecord.BillingAddress2) ? orderdetailrecord.BillingAddress2 : string.Empty;

        txtBillingZip.Text = orderdetailrecord.BillingZip;
        billingZipSearch_Click(this, EventArgs.Empty);

        ddlBillingCity.ClearSelection();
        ddlBillingCity.Items.FindByText(orderdetailrecord.BillingCity).Selected = true;
        ddlBillingState.ClearSelection();
        ddlBillingState.Items.FindByText(orderdetailrecord.BillingState).Selected = true;

        if ((orderdetailrecord.ServiceAddress == orderdetailrecord.BillingAddress) &&
             (orderdetailrecord.ServiceAddress2 == orderdetailrecord.BillingAddress2) &&
             (orderdetailrecord.ServiceZip == orderdetailrecord.BillingZip) &&
             (orderdetailrecord.ServiceCity == orderdetailrecord.BillingCity) &&
             (orderdetailrecord.ServiceState == orderdetailrecord.BillingState))
        { chkMakeSameAs.Checked = true; }
        else
        { chkMakeSameAs.Checked = false; }

        //Gas section is not populated since we do not store the UtilityChoice in the DB when on New York
        //There is no way to populate that as the schema currently exists.
        //ddlUtilityChoice_SelectedIndexChanged(this, EventArgs.Empty);
        //ddlGasContractTerm.ClearSelection();
        //ddlGasContractTerm.Items.FindByValue(mainrecord.ContractTermId.ToString()).Selected = true; 
        //divGas.Visible = false;
        // divModalGas.Visible = false;

        if ((ddlMarketState.SelectedItem.Text.ToLower() == "ny") || (ddlMarketState.SelectedItem.Text.ToLower() == "ma") || (ddlMarketState.SelectedItem.Text.ToLower() == "nj"))
        {
            //    //ddlMarketState.ClearSelection();
            //    //ddlMarketState.Items.FindByValue(mainrecord.MarketStateId.ToString()).Selected = true;
            //    //ddlMarketState_SelectedIndexChanged(this, EventArgs.Empty);

            //    ddlGasMarketUtility.ClearSelection();
            //    ddlGasMarketUtility.Items.FindByValue(mainrecord.GasMarketUtilityId.ToString()).Selected = true;
            //    ddlGasMarketUtility_SelectedIndexChanged(this, EventArgs.Empty);

            //    ResetMarketDropDowns();           
            ddlGasContractTerm.Items.FindByValue(mainrecord.GasContractTermId.ToString()).Selected = true;
        }



    }
    private void initFormFromServiceZipCodeSearch(string ZipCode)
    {
        //reset drop downs
        ddlServiceCity.Items.Clear();
        ddlServiceState.Items.Clear();

        List<string> ServiceCities = new List<string>();
        List<string> ServiceStates = new List<string>();


        using (LibertyEntities entities = new LibertyEntities())
        {
            ServiceCities = (from z in entities.ZipCodeLookups
                             where z.ZipCode == ZipCode
                             select z.City).ToList();

        }
        if (ServiceCities.Count > 0)
        {
            //populate ddlServiceCity
            foreach (string city in ServiceCities)
            {
                ddlServiceCity.Items.Add(new ListItem(city, city));
            }
            ddlServiceCity.Items.Insert(0, new ListItem("<-Select City->", ""));
        }

        using (LibertyEntities entities = new LibertyEntities())
        {
            ServiceStates = (from z in entities.ZipCodeLookups
                             where z.ZipCode == ZipCode
                             select z.State).Distinct().ToList();

        }
        if (ServiceStates.Count > 0)
        {
            //populate ddlServiceState
            foreach (string state in ServiceStates)
            {
                ddlServiceState.Items.Add(new ListItem(state, state));
            }
            ddlServiceState.Items.Insert(0, new ListItem("<-Select State->", ""));
        }


    }
    private void initFormFromBillingZipCodeSearch(string ZipCode)
    {
        //reset drop downs
        ResetBillingDropDowns();

        List<string> BillingCities = new List<string>();
        List<string> BillingStates = new List<string>();


        using (LibertyEntities entities = new LibertyEntities())
        {
            BillingCities = (from z in entities.ZipCodeLookups
                             where z.ZipCode == ZipCode
                             select z.City).ToList();

        }
        //populate ddlBillingCity
        if (BillingCities.Count > 0)
        {
            foreach (string city in BillingCities)
            {
                ddlBillingCity.Items.Add(new ListItem(city, city));
            }
            ddlBillingCity.Items.Insert(0, new ListItem("<-Select City->", ""));
        }

        using (LibertyEntities entities = new LibertyEntities())
        {
            BillingStates = (from z in entities.ZipCodeLookups
                             where z.ZipCode == ZipCode
                             select z.State).Distinct().ToList();

        }
        if (BillingStates.Count > 0)
        {
            //populate ddlBillingState
            foreach (string state in BillingStates)
            {
                ddlBillingState.Items.Add(new ListItem(state, state));
            }
            ddlBillingState.Items.Insert(0, new ListItem("<-Select City->", ""));
        }


    }

    private void ResetBillingDropDowns()
    {
        ddlBillingCity.Items.Clear();
        ddlBillingState.Items.Clear();
    }
    private void ResetMarketDropDowns()
    {
        ddlMarketUtility.Items.Clear();
        ddlGasMarketUtility.Items.Clear();
        ddlMarketProduct.Items.Clear();
        ddlContractTerm.Items.Clear();
        ResetDeliveryZoneDropDown();

        //Reset the divSubTermRate
        divSubTermRate.Visible = false;
        ResetDivSubTermRateControls();
    }

    /// <summary>
    /// Resets all controls for the Gas Form on main form and popup modal window
    /// </summary>
    private void ResetGasForm()
    {
        ResetDivGas();
        //ResetModalDivGas();
    }
    private void ResetDivGas()
    {
        txtGasLastNameCheck.Text = string.Empty;
        //ddlGasMarketUtility.Items.Clear();
        ddlGasContractTerm.SelectedIndex = 0;
        txtAccountNumber.Text = string.Empty;
        txtGasAccountNumber.Text = string.Empty;
        txtGasRate.Text = string.Empty;
        txtGasRateEffectiveDate.Text = string.Empty;
        txtBusinessTaxId.Text = string.Empty;
        txtBusinessName.Text = string.Empty;
        ddlSohoAccount.SelectedIndex = 0;
        txtRate.Text = string.Empty;
        txtRateEffectiveDate.Text = string.Empty;
    }
    //private void ResetModalDivGas()
    //{
    //    txtModalGasAccountNumber.Text = string.Empty;
    //}

    private void ResetDeliveryZoneDropDown()
    {
        ddlDeliveryZone.Items.Clear();
        divDeliveryZone.Visible = false;
    }
    private void ResetEffectiveRateValidation()
    {
        //1.	Remove both rules from Texas: They often have same day starts
        //2.	Remove the “Past date cannot be entered” from all other markets : Our agents often sell “Same Month” Rates.  
        if (ddlMarketState.SelectedItem.Text.ToLower() == "tx")
        {
            //CompareRateEffectiveDateTodayValidator.Enabled = false;
            regexRateEffectiveDate.Enabled = false;
        }
        else
        {
            //CompareRateEffectiveDateTodayValidator.Enabled = false;
            regexRateEffectiveDate.Enabled = true;
        }
    }

    //Build Gas and Electric Market Utilities Drop Down
    private void BuildMarketUtilitiesDropDown(int? MarketStateId)
    {
        List<MarketUtility> MarketUtilities = new List<MarketUtility>();
        using (LibertyEntities entities = new LibertyEntities())
        {
            MarketUtilities = (from mu in entities.MarketUtilities
                               where mu.MarketStateId == MarketStateId
                                && mu.Active == true
                                && mu.IsElectric == true
                               select mu).ToList();

        }
        if (MarketUtilities.Count > 0)
        {
            //populate Electric ddlMarketUtility
            foreach (MarketUtility item in MarketUtilities)
            {
                ddlMarketUtility.Items.Add(new ListItem(item.UtilityName, item.MarketUtilityId.ToString()));
            }
            ddlMarketUtility.Items.Insert(0, new ListItem("<-Select Utility->", ""));


        }

        if ((ddlMarketState.SelectedItem.Text.ToLower() == "ny") || (ddlMarketState.SelectedItem.Text.ToLower() == "ma") || (ddlMarketState.SelectedItem.Text.ToLower() == "nj"))
        {

            List<MarketUtility> GasMarketUtilities = new List<MarketUtility>();
            using (LibertyEntities entities = new LibertyEntities())
            {
                GasMarketUtilities = (from mu in entities.MarketUtilities
                                      where mu.MarketStateId == MarketStateId
                                       && mu.Active == true
                                       && mu.IsGas == true
                                      select mu).ToList();

            }
            if (GasMarketUtilities.Count > 0)
            {
                //populate ddlGasMarketUtility
                foreach (MarketUtility item in GasMarketUtilities)
                {
                    ddlGasMarketUtility.Items.Add(new ListItem(item.UtilityName, item.MarketUtilityId.ToString()));
                }
                ddlGasMarketUtility.Items.Insert(0, new ListItem("<-Select Utility->", ""));
            }
        }
    }
    private void BuildMarketProductsDropDown(int MarketStateId)
    {
        ddlMarketProduct.Items.Clear();
        List<MarketProduct> MarketProducts = new List<MarketProduct>();
        using (LibertyEntities entities = new LibertyEntities())
        {
            MarketProducts = (from mp in entities.MarketProducts
                              where mp.MarketStateId == MarketStateId
                                && mp.Active == true
                              select mp).OrderBy(x => x.ProductWebForm).ToList();

        }
        if (MarketProducts.Count > 0)
        {
            //populate ddlMarketProduct
            foreach (MarketProduct item in MarketProducts)
            {
                ddlMarketProduct.Items.Add(new ListItem(item.ProductWebForm, item.MarketProductId.ToString()));
            }
            ddlMarketProduct.Items.Insert(0, new ListItem("<-Select Product->", ""));
        }
    }

    //used to clear out NameKey controls and reset Jquery
    private void ResetNameKeyControls()
    {

        //Clear Jquery Mask for NameKey controls
        ClientScript.RegisterStartupScript(typeof(ScriptManager), "NameKeyScript", "", false);

        //MainForm
        txtNameKey.Text = string.Empty;
        divNameKeyControl.Visible = false;
        rfvNameKey.Enabled = false;

        //ModalForm
        //txtModalNameKey.Text = string.Empty;
        //divModalNameKeyControl.Visible = false;
        //rfvModalNameKey.Enabled = false;
    }
    //used to clear out ServiceNumber controls and reset Jquery
    private void ResetServiceNumberControls()
    {

        //Clear Jquery Mask for ServiceNumber controls
        ClientScript.RegisterStartupScript(typeof(ScriptManager), "ServiceNumberScript", "", false);

        //MainForm
        txtServiceNumber.Text = string.Empty;
        divServiceNumberControl.Visible = false;
        rfvServiceNumber.Enabled = false;

        //ModalForm
        //txtModalServiceNumber.Text = string.Empty;
        //divModalServiceNumberControl.Visible = false;
        //rfvModalServiceNumber.Enabled = false;
    }
    private void ResetAccountNumberControls()
    {

        //Clear Jquery Mask for ServiceNumber controls
        ClientScript.RegisterStartupScript(typeof(ScriptManager), "AccountNumberScript", "", false);

        //MainForm
        txtAccountNumber.Text = string.Empty;

        //ModalForm
        //txtModalAccountNumber.Text = string.Empty;
    }
    private void ResetGasAccountNumberControls()
    {

        //Clear Jquery Mask for ServiceNumber controls
        ClientScript.RegisterStartupScript(typeof(ScriptManager), "GasAccountNumberScript", "", false);

        //MainForm
        txtGasAccountNumber.Text = string.Empty;

        //ModalForm
        //txtGasAccountNumber.Text = string.Empty;
    }
    //used to clear out MeterNumber controls and reset Jquery
    private void ResetMeterNumberControls()
    {

        //Clear Jquery Mask for NameKey controls
        ClientScript.RegisterStartupScript(typeof(ScriptManager), "MeterNumberScript", "", false);

        //MainForm
        txtMeterNumber.Text = string.Empty;
        divMeterNumberControl.Visible = false;
        rfvMeterNumber.Enabled = false;
    }
    //reset divSubTermRate text controls
    private void ResetDivSubTermRateControls()
    {
        txtSubTermMonth1Start.Text = string.Empty;
        txtSubTermMonth1End.Text = string.Empty;
        txtSubTermMonth1Rate.Text = string.Empty;
        txtSubTermMonth2Start.Text = string.Empty;
        txtSubTermMonth2End.Text = string.Empty;
        txtSubTermMonth2Rate.Text = string.Empty;
        txtSubTermMonth3Start.Text = string.Empty;
        txtSubTermMonth3End.Text = string.Empty;
        txtSubTermMonth3Rate.Text = string.Empty;
        txtSubTermMonth4Start.Text = string.Empty;
        txtSubTermMonth4End.Text = string.Empty;
        txtSubTermMonth4Rate.Text = string.Empty;
    }
    //used to set up AccountNumber, NameKey and ServiceNumber controls with Jquery on postbacks
    private void SetUpDynamicJqueryControls()
    {

        if (!String.IsNullOrEmpty(ddlMarketUtility.SelectedValue.ToString()))
        {
            int MarketUtilityId = int.Parse(ddlMarketUtility.SelectedValue);
            //Get NameKey and ServiceReference Flags
            MarketUtility MarketUtility = new MarketUtility();

            //Build Electric Market Utility ddlMarketUtility from DB
            using (LibertyEntities entities = new LibertyEntities())
            {
                MarketUtility = (from mu in entities.MarketUtilities
                                 where mu.MarketUtilityId == MarketUtilityId
                                    && mu.Active == true
                                    && mu.IsElectric == true
                                 select mu).FirstOrDefault();

            }

            //Check Flags to turn on Mask for Electric Account Number Controls
            if (MarketUtility.AccountDigits != null)
            {
                //build Alphanumeric Mask definition based on digits required Electric
                StringBuilder sbMaskDefinition = new StringBuilder();
                int? maskDigitCount = null;
                string MaskDefinitionDeclaration = "A";
                maskDigitCount = MarketUtility.AccountDigits ?? 0;
                for (int i = 0; i < maskDigitCount; i++)
                {
                    sbMaskDefinition.Append(MaskDefinitionDeclaration);
                }

                //StringBuilder object for dynamic jquery mask creation
                StringBuilder strScript = new StringBuilder();
                strScript.Append("$(document).ready(function(){");
                strScript.Append("$.mask.definitions['" + MaskDefinitionDeclaration + "'] = '" + MarketUtility.AccountMask + "';");
                strScript.Append("$('#" + txtAccountNumber.ClientID + "').mask('" + sbMaskDefinition + "', {autoclear: false});");
                //strScript.Append("$('#" + txtModalAccountNumber.ClientID + "').mask('" + sbMaskDefinition + "');");

                strScript.Append("});");
                //Register the strScript to the ScriptManager ont he UpdatePanel
                ScriptManager.RegisterStartupScript(((UpdatePanel)Master.FindControl("UpdatePanel1")), ((UpdatePanel)Master.FindControl("UpdatePanel1")).GetType(), "AccountNumberScript", strScript.ToString(), true);

                //rfvAccountNumber.Text = maskDigitCount + " digit Account Number is Required.";

                regexAccountNumber.Enabled = true;
                //regexAccountNumber.ErrorMessage = maskDigitCount + " digit Account Number is Required.";
                regexAccountNumber.ValidationExpression = MarketUtility.AccountMask + @"{0," + maskDigitCount + "}";
                lblAccountNumber.CssClass = "has-error";
                lblAccountNumber.Text = "<img alt=\"!\" src=\"images/validationerror.png\" /> <B>" + maskDigitCount + " digit Account Number is Required.</B>";


            }
            else
            {
                ResetAccountNumberControls();
            }


            //check Flags to turn on div and validation For Name Key controls
            if (MarketUtility.NameKey == true)
            {
                //build Alphanumeric Mask definition based on digits required
                StringBuilder sbMaskDefinition = new StringBuilder();
                int? maskDigitCount = null;
                string MaskDefinitionDeclaration = "A";
                maskDigitCount = MarketUtility.NameKeyDigits ?? 0;
                for (int i = 0; i < maskDigitCount; i++)
                {
                    sbMaskDefinition.Append(MaskDefinitionDeclaration);
                }


                //MainForm
                divNameKeyControl.Visible = true;
                rfvNameKey.Enabled = true;

                //ModalForm
                //divModalNameKeyControl.Visible = true;
                //rfvModalNameKey.Enabled = true;

                //StringBuilder object for dynamic jquery mask creation
                StringBuilder strScript = new StringBuilder();
                strScript.Append("$(document).ready(function(){");
                strScript.Append("$.mask.definitions['" + MaskDefinitionDeclaration + "'] = '" + MarketUtility.NameKeyMask + "';");
                strScript.Append("$('#" + txtNameKey.ClientID + "').mask('" + sbMaskDefinition + "');");
                //strScript.Append("$('#" + txtModalNameKey.ClientID + "').mask('" + sbMaskDefinition + "');");
                strScript.Append("});");
                //Register the strScript to the ScriptManager ont he UpdatePanel
                ScriptManager.RegisterStartupScript(((UpdatePanel)Master.FindControl("UpdatePanel1")), ((UpdatePanel)Master.FindControl("UpdatePanel1")).GetType(), "NameKeyScript", strScript.ToString(), true);

            }
            else
            {
                ResetNameKeyControls();
            }

            //check Flags to turn on div and validation For Service Reference controls
            if (MarketUtility.ServiceReference == true)
            {
                //build Numberic Mask definition based on digits required
                StringBuilder sbMaskDefinition = new StringBuilder();
                int? maskDigitCount = null;
                string MaskDefinitionDeclaration = MarketUtility.ServiceReferenceMask;
                maskDigitCount = MarketUtility.ServiceReferenceDigits ?? 0;
                for (int i = 0; i < maskDigitCount; i++)
                {
                    sbMaskDefinition.Append(MaskDefinitionDeclaration);
                }

                //MainForm
                divServiceNumberControl.Visible = true;
                rfvServiceNumber.Enabled = true;

                //ModalForm
                //divModalServiceNumberControl.Visible = true;
                //rfvModalServiceNumber.Enabled = true;

                //StringBuilder object for dynamic jquery mask creation
                StringBuilder strScript = new StringBuilder();
                strScript.Append("$(document).ready(function(){");
                strScript.Append("$('#" + txtServiceNumber.ClientID + "').mask('" + sbMaskDefinition + "');");
                //strScript.Append("$('#" + txtModalServiceNumber.ClientID + "').mask('" + sbMaskDefinition + "');");
                strScript.Append("});");
                //Register the strScript to the ScriptManager ont he UpdatePanel
                ScriptManager.RegisterStartupScript(((UpdatePanel)Master.FindControl("UpdatePanel1")), ((UpdatePanel)Master.FindControl("UpdatePanel1")).GetType(), "ServiceNumberScript", strScript.ToString(), true);

            }
            else
            {
                ResetServiceNumberControls();
            }

        }

        if (!String.IsNullOrEmpty(ddlGasMarketUtility.SelectedValue.ToString()))
        {
            int GasMarketUtilityId = int.Parse(ddlGasMarketUtility.SelectedValue);
            //Get NameKey and ServiceReference Flags
            MarketUtility GasMarketUtility = new MarketUtility();


            using (LibertyEntities entities = new LibertyEntities())
            {
                GasMarketUtility = (from mu in entities.MarketUtilities
                                    where mu.MarketUtilityId == GasMarketUtilityId
                                       && mu.Active == true
                                       && mu.IsGas == true
                                    select mu).FirstOrDefault();

            }
            //Check Flags to turn on Mask for Gas Account Number Controls
            if (GasMarketUtility.AccountDigits != null)
            {
                //build Alphanumeric Mask definition based on digits required
                StringBuilder sbMaskGasDefinition = new StringBuilder();
                int? maskGasDigitCount = null;
                string MaskGasDefinitionDeclaration = "A";
                maskGasDigitCount = GasMarketUtility.AccountDigits ?? 0;
                for (int i = 0; i < maskGasDigitCount; i++)
                {
                    sbMaskGasDefinition.Append(MaskGasDefinitionDeclaration);
                }

                //StringBuilder object for dynamic jquery mask creation
                StringBuilder strScript = new StringBuilder();
                strScript.Append("$(document).ready(function(){");
                strScript.Append("$.mask.definitions['" + MaskGasDefinitionDeclaration + "'] = '" + GasMarketUtility.AccountMask + "';");
                strScript.Append("$('#" + txtGasAccountNumber.ClientID + "').mask('" + sbMaskGasDefinition + "', {autoclear: false});");
                //strScript.Append("$('#" + txtModalAccountNumber.ClientID + "').mask('" + sbMaskDefinition + "');");

                if (ddlUtilityChoice.SelectedItem.Text.ToLower() != "electric")
                {
                    strScript.Append("$('#" + txtGasAccountNumber.ClientID + "').mask('" + sbMaskGasDefinition + "', {autoclear: false});");
                    //strScript.Append("$('#" + txtModalGasAccountNumber.ClientID + "').mask('" + sbMaskGasDefinition + "');");

                    strScript.Append("});");
                    //Register the strScript to the ScriptManager ont he UpdatePanel
                    ScriptManager.RegisterStartupScript(((UpdatePanel)Master.FindControl("UpdatePanel1")), ((UpdatePanel)Master.FindControl("UpdatePanel1")).GetType(), "GASAccountNumberScript", strScript.ToString(), true);

                    regexGasAccountNumber.Enabled = true;
                    regexGasAccountNumber.ValidationExpression = GasMarketUtility.AccountMask + @"{0," + maskGasDigitCount + "}";
                    lblGasAccountNumber.CssClass = "has-error";
                    lblGasAccountNumber.Text = "<img alt=\"!\" src=\"images/validationerror.png\" /> <B>" + maskGasDigitCount + " digit Gas Account Number is Required.</B>";

                    //regexModalGasAccountNumber.Enabled = true;
                    //regexModalGasAccountNumber.ValidationExpression = GasMarketUtility.AccountMask + @"{0," + maskGasDigitCount + "}";
                    //lblModalGasAccountNumber.CssClass = "has-error";
                    //lblModalGasAccountNumber.Text = "<img alt=\"!\" src=\"images/validationerror.png\" /> <B>" + maskGasDigitCount + " digit Gas Account Number is Required.</B>";
                }
            }
            else
            {
                ResetGasAccountNumberControls();
            }

            //check Flags to turn on div and validation For Service Reference controls
            if (GasMarketUtility.MeterNumber == true)
            {
                //build AlphaNumeric Mask definition based on digits required
                StringBuilder sbMaskDefinition = new StringBuilder();
                int? maskDigitCount = null;
                string MaskDefinitionDeclaration = "A";
                maskDigitCount = GasMarketUtility.MeterNumberDigits ?? 0;
                for (int i = 0; i < maskDigitCount; i++)
                {
                    sbMaskDefinition.Append(MaskDefinitionDeclaration);
                }

                //MainForm
                divMeterNumberControl.Visible = true;
                rfvMeterNumber.Enabled = true;

                //StringBuilder object for dynamic jquery mask creation
                StringBuilder strScript = new StringBuilder();
                strScript.Append("$(document).ready(function(){");
                strScript.Append("$.mask.definitions['" + MaskDefinitionDeclaration + "'] = '" + GasMarketUtility.MeterNumberMask + "';");
                strScript.Append("$('#" + txtMeterNumber.ClientID + "').mask('?" + sbMaskDefinition + "');"); //allow up to the number of digits in the db

                strScript.Append("});");
                //Register the strScript to the ScriptManager on the UpdatePanel
                ScriptManager.RegisterStartupScript(((UpdatePanel)Master.FindControl("UpdatePanel1")), ((UpdatePanel)Master.FindControl("UpdatePanel1")).GetType(), "MeterNumberScript", strScript.ToString(), true);

            }
            else
            {
                ResetMeterNumberControls();
            }


        }
    }

    #endregion

    #region Form Controls

    protected void phoneSearch_Click(object sender, EventArgs e)
    {
        if (txtPhoneNumber.Text.Trim() != "")
        {
            Main mainRecord = new Main();
            string btn = StripAllNonNumerics(txtPhoneNumber.Text.Trim());

            mainRecord = GetMainBasedOnBtn(btn);

            if (mainRecord != null)
            {
                initFormFromPhoneSearch(mainRecord);
            }
        }

        SetUpDynamicJqueryControls();
    }
    protected void ddlMarketState_SelectedIndexChanged(object sender, EventArgs e)
    {
        ResetMarketDropDowns();
        ResetNameKeyControls();
        ResetAccountNumberControls();
        ResetGasAccountNumberControls();
        ResetServiceNumberControls();
        ResetMeterNumberControls();

        int MarketStateId = int.Parse(ddlMarketState.SelectedValue);
        BuildMarketUtilitiesDropDown(MarketStateId);

        //ResetEffectiveRateValidation();//may not be necessary for it to be here since the MarketUtility change happens below

        //Capture EnergyGRT if Market State is PA
        if (ddlMarketState.SelectedItem.Text.ToLower() == "pa")
        {
            divEnergyGRT.Visible = true;
        }
        else
        {
            divEnergyGRT.Visible = false;
            txtEnergyGRT.Text = string.Empty;
        }

        if (ddlMarketState.SelectedItem.Text.ToLower() == "tx")
        {
            divContractId.Visible = true;
            divEFLVersionCode.Visible = true;
            divSwitchOrMoveIn.Visible = true;
            divMaterialPreference.Visible = true;
        }
        else
        {
            divContractId.Visible = false;
            txtContractId.Text = string.Empty;

            divEFLVersionCode.Visible = false;
            txtEFLVersionCode.Text = string.Empty;

            divSwitchOrMoveIn.Visible = false;
            ddlSwitchOrMoveIn.SelectedIndex = 0;

            divMaterialPreference.Visible = false;
            ddlMaterialPreference.SelectedIndex = 0;
        }

        if ((ddlMarketState.SelectedItem.Text.ToLower() == "tx") || (ddlMarketState.SelectedItem.Text.ToLower() == "oh"))
        {
            divRateExpirationDate.Visible = true;
        }
        else
        {
            divRateExpirationDate.Visible = false;
            txtRateExpirationDate.Text = string.Empty;
        }

        ResetGasForm();//reset Gas Form Sections

        if ((ddlMarketState.SelectedItem.Text.ToLower() == "ny") || (ddlMarketState.SelectedItem.Text.ToLower() == "ma") || (ddlMarketState.SelectedItem.Text.ToLower() == "nj"))
        {
            //turn on the ddlUtilityTypeChoice
            divUtilityChoice.Visible = true;
            ddlUtilityChoice.SelectedIndex = 0;

            //show Div Gas Section
            divGas.Visible = true;

            //show Modal Div Gas Section
            //divModalGas.Visible = true;
        }
        else
        {
            //turn off the ddlUtilityChoice
            divUtilityChoice.Visible = false;
            ddlUtilityChoice.SelectedIndex = 0;

            //Hide Gas Form Sections
            divGas.Visible = false;

            //Turn on Electric sections
            divElectricUtility.Visible = true;
            divElectricMarketProduct.Visible = true;
            divElectricBusiness.Visible = true;
            divElectricAccountNumber.Visible = true;
            divElectricServiceSection.Visible = true;

            //Hide Modal Div Gas Section
            //divModalGas.Visible = false;
        }

        SetUpDynamicJqueryControls();
    }

    //Electric, Gas or Electric & Gas dropdown
    protected void ddlUtilityChoice_SelectedIndexChanged(object sender, EventArgs e)
    {

        ResetGasForm();//reset Gas Form Sections


        if (ddlUtilityChoice.SelectedItem.Text.ToLower() == "gas")
        {
            //Turn off Electric sections
            divElectricUtility.Visible = false;
            divElectricMarketProduct.Visible = false;
            divElectricBusiness.Visible = false;
            divElectricAccountNumber.Visible = false;
            divElectricServiceSection.Visible = false;

            divGas.Visible = true;

        }
        else if (ddlUtilityChoice.SelectedItem.Text.ToLower() == "electric")
        {
            //Turn on Electric sections
            divElectricUtility.Visible = true;
            divElectricMarketProduct.Visible = true;
            divElectricBusiness.Visible = true;
            divElectricAccountNumber.Visible = true;
            divElectricServiceSection.Visible = true;

            divGas.Visible = false;

        }
        else//electric & gas
        {
            //Turn on Electric sections
            divElectricUtility.Visible = true;
            divElectricMarketProduct.Visible = true;
            divElectricBusiness.Visible = true;
            divElectricAccountNumber.Visible = true;
            divElectricServiceSection.Visible = true;

            divGas.Visible = true;

        }
        ResetMarketDropDowns();

        int MarketStateId = int.Parse(ddlMarketState.SelectedValue);
        BuildMarketUtilitiesDropDown(MarketStateId);
    }

    protected void ddlMarketProduct_SelectedIndexChanged(object sender, EventArgs e)
    {
        ddlContractTerm.Items.Clear();


        //need to do deliveryzone
        ResetDeliveryZoneDropDown();

        int MarketProductId = int.Parse(ddlMarketProduct.SelectedValue);

        List<ContractTerm> ContractTerms = new List<ContractTerm>();

        using (LibertyEntities entities = new LibertyEntities())
        {
            ContractTerms = (from ct in entities.ContractTerms
                             join pcl in entities.ProductContractLinks on ct.ContractTermId equals pcl.ContractTermId
                             where pcl.MarketProductId == MarketProductId
                             select ct).OrderBy(x => x.ContractTermId).ToList();

        }
        if (ContractTerms.Count > 0)
        {
            //populate ddlMarketContractTerm
            foreach (ContractTerm item in ContractTerms)
            {
                ddlContractTerm.Items.Add(new ListItem(item.MonthlyTerm, item.ContractTermId.ToString()));
            }
            ddlContractTerm.Items.Insert(0, new ListItem("<-Select Term of Service->", ""));


        }

        //Get Commercial Flag
        MarketProduct MarketProduct = new MarketProduct();
        using (LibertyEntities entities = new LibertyEntities())
        {
            MarketProduct = (from mp in entities.MarketProducts
                             where mp.MarketProductId == MarketProductId
                               && mp.Active == true
                             select mp).FirstOrDefault();

        }

        //check commercial flag to turn on business name div and validation
        if (MarketProduct.Commercial == true)
        {
            divBusinessNameLabel.Visible = true;
            divBusinessNameControl.Visible = true;
            rfvBusinessName.Enabled = true;
        }
        else
        {
            divBusinessNameLabel.Visible = false;
            divBusinessNameControl.Visible = false;
            rfvBusinessName.Enabled = false;
        }

        //check SubTermRate flag to toggle Rate or SubTermRate divs
        if (MarketProduct.SubTermRate == true)
        {
            divRate.Visible = false;
            txtRate.Text = string.Empty;

            divSubTermRate.Visible = true;
            //divModalSubTermRate.Visible = true;
        }
        else
        {
            divRate.Visible = true;

            //Reset the divSubTermRate
            divSubTermRate.Visible = false;
            ResetDivSubTermRateControls();


            //divModalSubTermRate.Visible = false;
            //txtModalSubTermMonth1Rate.Text = string.Empty;
            //txtModalSubTermMonth2Rate.Text = string.Empty;
            //txtModalSubTermMonth3Rate.Text = string.Empty;
            //txtModalSubTermMonth4Rate.Text = string.Empty;
        }

        //Controls the ddlDeliveryZone 
        if (MarketProductId == 200) //State:MA MarketProduct: Green(Wind) Falling Prices - Residential
        {
            divDeliveryZone.Visible = true;

            List<DeliveryZone> DeliveryZone = new List<DeliveryZone>();

            using (LibertyEntities entities = new LibertyEntities())
            {
                DeliveryZone = (from dz in entities.DeliveryZones
                                select dz).OrderBy(x => x.Name).ToList();

            }
            if (DeliveryZone.Count > 0)
            {
                //populate ddlDeliveryZone
                foreach (DeliveryZone item in DeliveryZone)
                {
                    ddlDeliveryZone.Items.Add(new ListItem(item.Name, item.DeliveryZoneId.ToString()));
                }
                ddlDeliveryZone.Items.Insert(0, new ListItem("<-Select Delivery Zone->", ""));
            }
        }
        //else //done everytime this method runs
        //{
        //    ResetDeliveryZoneDropDown();
        //}



        SetUpDynamicJqueryControls();
    }


    //Electric Market Utility drop down
    protected void ddlMarketUtility_SelectedIndexChanged(object sender, EventArgs e)
    {
        //reset values for textboxes
        txtNameKey.Text = string.Empty;
        txtServiceNumber.Text = string.Empty;

        //txtModalNameKey.Text = string.Empty;
        //txtModalServiceNumber.Text = string.Empty;

        //SetUpNameKeyAndServiceNumberControls();

        int MarketStateId = int.Parse(ddlMarketState.SelectedValue);
        BuildMarketProductsDropDown(MarketStateId);

        ResetEffectiveRateValidation();


        //need to do deliveryzone
        ResetDeliveryZoneDropDown();

        //Reset the divSubTermRate
        divSubTermRate.Visible = false;
        ResetDivSubTermRateControls();
    }

    //Gas Market Utility drop down
    protected void ddlGasMarketUtility_SelectedIndexChanged(object sender, EventArgs e)
    {
        SetUpDynamicJqueryControls();
    }
    protected void serviceZipSearch_Click(object sender, EventArgs e)
    {
        string serviceZip = StripAllNonNumerics(txtServiceZip.Text.Trim());

        ZipCodeLookup zipcodelookup = GetZipCodeLookup(serviceZip);

        //reset drop downs
        ddlServiceCity.Items.Clear();
        ddlServiceState.Items.Clear();

        if (zipcodelookup != null)
        {
            initFormFromServiceZipCodeSearch(serviceZip);
        }

        SetUpDynamicJqueryControls();
    }
    protected void billingZipSearch_Click(object sender, EventArgs e)
    {
        string billingZip = StripAllNonNumerics(txtBillingZip.Text.Trim());

        ZipCodeLookup zipcodelookup = GetZipCodeLookup(billingZip);

        //reset drop downs
        ResetBillingDropDowns();

        if (zipcodelookup != null)
        {
            initFormFromBillingZipCodeSearch(billingZip);
        }

        SetUpDynamicJqueryControls();
    }
    protected void chkMakeSameAs_CheckedChanged(object sender, EventArgs e)
    {
        // Make Billing address form same as Service address form

        if (chkMakeSameAs.Checked == true)
        {
            txtBillingAddress1.Text = txtServiceAddress1.Text;
            txtBillingAddress2.Text = txtServiceAddress2.Text;
            txtBillingZip.Text = txtServiceZip.Text;

            ////reset Billing drop downs
            ResetBillingDropDowns();

            //Need to pop the dropdown lists for City and State and choose the values
            initFormFromBillingZipCodeSearch(txtBillingZip.Text);

            ddlBillingCity.SelectedValue = ddlServiceCity.SelectedValue;
            ddlBillingState.SelectedValue = ddlServiceState.SelectedValue;
        }
        else
        {
            txtBillingAddress1.Text = string.Empty;
            txtBillingAddress2.Text = string.Empty;
            txtBillingZip.Text = string.Empty;
            //Need to pop the dropdown lists for City and State and choose the values
            initFormFromBillingZipCodeSearch(txtBillingZip.Text);
        }

        SetUpDynamicJqueryControls();
    }

    //Validates Form
    protected void btnSave_Click(object sender, EventArgs e)
    {
        //if (PerformValidation())
        // {

        //Save form data to Main session variable and one OrderDetail session, may have to make the OrderDetail a list.
        //may have to build a created object and not the EF object since some values will be null, 
        //specifically the PK and FK relationship on the MainId to the OrderDetail records
        MainFormRecord mainformrecord = new MainFormRecord();
        mainformrecord.UserId = int.Parse(SessionVars.AdminUser.UserId.ToString());
        mainformrecord.SalesChannelId = int.Parse(ddlSalesChannelId.SelectedItem.Value);
        mainformrecord.SalesAgentId = txtSalesAgentId.Text.Trim();
        mainformrecord.Btn = StripAllNonNumerics(txtPhoneNumber.Text.Trim());
        mainformrecord.MarketStateId = int.Parse(ddlMarketState.SelectedItem.Value);
        mainformrecord.MarketUtilityId = string.IsNullOrEmpty(ddlMarketUtility.SelectedValue) ? (int?)null : int.Parse(ddlMarketUtility.SelectedItem.Value);
        mainformrecord.MarketProductId = string.IsNullOrEmpty(ddlMarketProduct.SelectedValue) ? (int?)null : int.Parse(ddlMarketProduct.SelectedItem.Value);
        mainformrecord.DeliveryZoneId = string.IsNullOrEmpty(ddlDeliveryZone.SelectedValue) ? (int?)null : int.Parse(ddlDeliveryZone.SelectedItem.Value);
        mainformrecord.AuthorizationFirstName = string.IsNullOrEmpty(txtAuthorizationFirstName.Text) ? null : txtAuthorizationFirstName.Text.Trim();
        mainformrecord.AuthorizationLastName = string.IsNullOrEmpty(txtAuthorizationLasttName.Text) ? null : txtAuthorizationLasttName.Text.Trim();
        mainformrecord.Email = string.IsNullOrEmpty(txtEmail.Text) ? null : txtEmail.Text.Trim();
        mainformrecord.BusinessTaxId = string.IsNullOrEmpty(txtBusinessTaxId.Text) ? null : txtBusinessTaxId.Text.Trim();
        mainformrecord.BusinessName = string.IsNullOrEmpty(txtBusinessName.Text) ? null : txtBusinessName.Text.Trim();
        mainformrecord.SOHOAccount = ddlSohoAccount.SelectedIndex == 0 ? null : ddlSohoAccount.SelectedItem.Text;
        mainformrecord.ContractTermId = string.IsNullOrEmpty(ddlContractTerm.SelectedValue) ? (int?)null : int.Parse(ddlContractTerm.SelectedItem.Value);
        mainformrecord.EnergyGRT = string.IsNullOrEmpty(txtEnergyGRT.Text) ? null : txtEnergyGRT.Text.Trim();
        mainformrecord.ContractID = string.IsNullOrEmpty(txtContractId.Text) ? null : txtContractId.Text.Trim();
        mainformrecord.EFLVersionCode = string.IsNullOrEmpty(txtEFLVersionCode.Text) ? null : txtEFLVersionCode.Text.Trim();
        mainformrecord.SwitchOrMoveIn = ddlSwitchOrMoveIn.SelectedIndex == 0 ? null : ddlSwitchOrMoveIn.SelectedItem.Text;
        mainformrecord.MaterialPreference = ddlMaterialPreference.SelectedIndex == 0 ? null : ddlMaterialPreference.SelectedItem.Text;
        mainformrecord.EstDateExpiration = string.IsNullOrEmpty(txtRateExpirationDate.Text) ? null : txtRateExpirationDate.Text.Trim();
        mainformrecord.Rate = string.IsNullOrEmpty(txtRate.Text) ? null : txtRate.Text.Trim();
        mainformrecord.RateEffectiveDate = string.IsNullOrEmpty(txtRateEffectiveDate.Text) ? null : txtRateEffectiveDate.Text.Trim();
        mainformrecord.SubTermMonth1Start = string.IsNullOrEmpty(txtSubTermMonth1Start.Text) ? null : txtSubTermMonth1Start.Text.Trim();
        mainformrecord.SubTermMonth1End = string.IsNullOrEmpty(txtSubTermMonth1End.Text) ? null : txtSubTermMonth1End.Text.Trim();
        mainformrecord.SubTermMonth2Start = string.IsNullOrEmpty(txtSubTermMonth2Start.Text) ? null : txtSubTermMonth2Start.Text.Trim();
        mainformrecord.SubTermMonth2End = string.IsNullOrEmpty(txtSubTermMonth2End.Text) ? null : txtSubTermMonth2End.Text.Trim();
        mainformrecord.SubTermMonth3Start = string.IsNullOrEmpty(txtSubTermMonth3Start.Text) ? null : txtSubTermMonth3Start.Text.Trim();
        mainformrecord.SubTermMonth3End = string.IsNullOrEmpty(txtSubTermMonth3End.Text) ? null : txtSubTermMonth3End.Text.Trim();
        mainformrecord.SubTermMonth4Start = string.IsNullOrEmpty(txtSubTermMonth4Start.Text) ? null : txtSubTermMonth4Start.Text.Trim();
        mainformrecord.SubTermMonth4End = string.IsNullOrEmpty(txtSubTermMonth4End.Text) ? null : txtSubTermMonth4End.Text.Trim();
        mainformrecord.NumberOfAccounts = null;//txtNumOfAccounts.Text.Trim();

        mainformrecord.GasMarketUtilityId = string.IsNullOrEmpty(ddlGasMarketUtility.SelectedValue) ? (int?)null : int.Parse(ddlGasMarketUtility.SelectedItem.Value);
        mainformrecord.GasRate = string.IsNullOrEmpty(txtGasRate.Text) ? null : txtGasRate.Text.Trim();

        mainformrecord.GasContractTermId = string.IsNullOrEmpty(ddlGasContractTerm.SelectedValue) ? (int?)null : int.Parse(ddlGasContractTerm.SelectedItem.Value);
        mainformrecord.GasRateEffectiveDate = string.IsNullOrEmpty(txtGasRateEffectiveDate.Text) ? null : txtGasRateEffectiveDate.Text.Trim();

        List<OrderDetailFormRecord> orderformrecords = new List<OrderDetailFormRecord>();

        OrderDetailFormRecord orderformrecord = new OrderDetailFormRecord();
        orderformrecord.OrderDetailFormRecordNumber = 1;
        orderformrecord.AccountNumber = string.IsNullOrEmpty(txtAccountNumber.Text) ? null : txtAccountNumber.Text.Trim();
        orderformrecord.GasAccountNumber = string.IsNullOrEmpty(txtGasAccountNumber.Text) ? null : txtGasAccountNumber.Text.Trim();
        orderformrecord.MeterNumber = string.IsNullOrEmpty(txtMeterNumber.Text) ? null : txtMeterNumber.Text.Trim();
        orderformrecord.Btn = StripAllNonNumerics(txtPhoneNumber.Text.Trim());
        orderformrecord.NameKey = string.IsNullOrEmpty(txtNameKey.Text) ? null : txtNameKey.Text.Trim();
        orderformrecord.ServiceNumber = string.IsNullOrEmpty(txtServiceNumber.Text) ? null : txtServiceNumber.Text.Trim();
        orderformrecord.SubTermRate1 = string.IsNullOrEmpty(txtSubTermMonth1Rate.Text) ? null : txtSubTermMonth1Rate.Text.Trim();
        orderformrecord.SubTermRate2 = string.IsNullOrEmpty(txtSubTermMonth2Rate.Text) ? null : txtSubTermMonth2Rate.Text.Trim();
        orderformrecord.SubTermRate3 = string.IsNullOrEmpty(txtSubTermMonth3Rate.Text) ? null : txtSubTermMonth3Rate.Text.Trim();
        orderformrecord.SubTermRate4 = string.IsNullOrEmpty(txtSubTermMonth4Rate.Text) ? null : txtSubTermMonth4Rate.Text.Trim();
        orderformrecord.ServiceAddress1 = string.IsNullOrEmpty(txtServiceAddress1.Text) ? null : txtServiceAddress1.Text.Trim();
        orderformrecord.ServiceAddress2 = string.IsNullOrEmpty(txtServiceAddress2.Text) ? null : txtServiceAddress2.Text.Trim();
        orderformrecord.ServiceCity = ddlServiceCity.SelectedIndex == 0 ? null : ddlServiceCity.SelectedItem.Text;
        orderformrecord.ServiceState = ddlServiceState.SelectedIndex == 0 ? null : ddlServiceState.SelectedItem.Text;
        orderformrecord.ServiceZip = string.IsNullOrEmpty(txtServiceZip.Text) ? null : txtServiceZip.Text.Trim();
        orderformrecord.BillingAddress1 = string.IsNullOrEmpty(txtBillingAddress1.Text) ? null : txtBillingAddress1.Text.Trim();
        orderformrecord.BillingAddress2 = string.IsNullOrEmpty(txtBillingAddress2.Text) ? null : txtBillingAddress2.Text.Trim();
        orderformrecord.BillingCity = ddlBillingCity.SelectedIndex == 0 ? null : ddlBillingCity.SelectedItem.Text;
        orderformrecord.BillingState = ddlBillingState.SelectedIndex == 0 ? null : ddlBillingState.SelectedItem.Text;
        orderformrecord.BillingZip = string.IsNullOrEmpty(txtBillingZip.Text) ? null : txtBillingZip.Text.Trim();

        orderformrecords.Add(orderformrecord);

        mainformrecord.OrderDetailFormRecords = orderformrecords;

        SessionVars.MainFormRecord = mainformrecord;

        //turn on Submit button
        btnSubmit.Visible = true;

        //int howManyAccounts = int.Parse(txtNumOfAccounts.Text);
        //if (howManyAccounts > 1)
        //{
        //    //Set ModalForm to Add mode
        //    SessionVars.AccountEditMode = false;

        //    //need to pop modal form
        //    mpePopUp.Show();

        //}

        SetUpDynamicJqueryControls();
    }

    //Submits MainFormRecord and its OrderDetailFormRecord objects to the DB
    protected void btnSubmit_Click(object sender, EventArgs e)
    {

        //Need to Grab SessionVars.MainFormRecord <-has at least one OrderDetailFormRecord
        MainFormRecord MainFormRecord = new MainFormRecord();

        MainFormRecord = (MainFormRecord)SessionVars.MainFormRecord;

        //Need to Grab SessionVars.List<OrderDetailFormRecord) if not null

        //List<> of Records
        List<OrderDetailFormRecord> listOfOrderformrecords = new List<OrderDetailFormRecord>();

        //grab SessionVars.OrderDetailFormRecordList
        if (SessionVars.OrderDetailFormRecordList != null)
        {
            listOfOrderformrecords = (List<OrderDetailFormRecord>)SessionVars.OrderDetailFormRecordList;
        }
        //Need to add SessionVars.List<OrderDetailFormRecord) contents to SessionVars.MainFormRecord.OrderDetailFormRecord in order

        foreach (OrderDetailFormRecord selectedRecord in listOfOrderformrecords)
        {
            MainFormRecord.OrderDetailFormRecords.Add(selectedRecord);
        }


        //submit information to the db       
        SessionVars.CurrentAccount = InsertRecord(MainFormRecord);

        //redirect to display page for RecordLocator
        Response.Redirect("Display.aspx");
    }
    #endregion

    #region Entity Framework Interaction (5 Methods)

    #region GetData (5 Methods)
    private Office GetOffice(int? OfficeId)
    {
        using (LibertyEntities entities = new LibertyEntities())
        {
            Office office = entities.Offices.FirstOrDefault(x => x.OfficeId == OfficeId && x.IsActive == true);
            return office;
        }
    }
    private Vendor GetVendor(int VendorId)
    {
        using (LibertyEntities entities = new LibertyEntities())
        {
            Vendor vendor = entities.Vendors.FirstOrDefault(x => x.VendorId == VendorId && x.IsActive == true);
            return vendor;
        }
    }
    private SalesChannel GetSalesChannel(int SalesChannelId)
    {
        using (LibertyEntities entities = new LibertyEntities())
        {
            SalesChannel salesChannel = entities.SalesChannels.FirstOrDefault(x => x.SalesChannelId == SalesChannelId && x.IsActive == true);
            return salesChannel;
        }
    }
    private Main GetMainBasedOnBtn(string Btn)
    {
        using (LibertyEntities entities = new LibertyEntities())
        {
            Main main = entities.Mains
                .Include("OrderDetails")
                .Where(x => x.Btn == Btn)
                .OrderByDescending(x => x.WebDateTime)
                .FirstOrDefault();
            return main;
        }
    }
    private ZipCodeLookup GetZipCodeLookup(string ZipCode)
    {
        using (LibertyEntities entities = new LibertyEntities())
        {
            ZipCodeLookup zipCodeRecords = entities.ZipCodeLookups.FirstOrDefault(x => x.ZipCode == ZipCode);
            return zipCodeRecords;
        }
    }
    #endregion

    #region InsertData (1 Method)
    private Main InsertRecord(MainFormRecord MainFormRecord)
    {
        Main main = null;
        using (LibertyEntities data = new LibertyEntities())
        {
            main = new Main();

            main.UserId = MainFormRecord.UserId;
            main.SalesChannelId = MainFormRecord.SalesChannelId;
            main.SalesAgentId = MainFormRecord.SalesAgentId;
            main.Btn = MainFormRecord.Btn;
            main.MarketStateId = MainFormRecord.MarketStateId;
            main.MarketUtilityId = MainFormRecord.MarketUtilityId;
            main.MarketProductId = MainFormRecord.MarketProductId;
            main.DeliveryZoneId = MainFormRecord.DeliveryZoneId;
            main.AuthorizationFirstName = MainFormRecord.AuthorizationFirstName;
            main.AuthorizationLastName = MainFormRecord.AuthorizationLastName;
            main.Email = MainFormRecord.Email;
            main.BusinessTaxId = MainFormRecord.BusinessTaxId;
            main.BusinessName = MainFormRecord.BusinessName;
            main.SOHOAccount = IsValueNull(MainFormRecord.SOHOAccount) ? null : MainFormRecord.SOHOAccount;
            main.ContractTermId = MainFormRecord.ContractTermId;
            main.EnergyGRT = MainFormRecord.EnergyGRT;

            main.ContractID = MainFormRecord.ContractID;
            main.EFLVersionCode = MainFormRecord.EFLVersionCode;
            main.SwitchOrMoveIn = MainFormRecord.SwitchOrMoveIn;
            main.MaterialPreference = MainFormRecord.MaterialPreference;
            main.EstDateExpiration = MainFormRecord.EstDateExpiration;

            main.Rate = MainFormRecord.Rate;
            main.RateEffectiveDate = MainFormRecord.RateEffectiveDate;
            main.SubTermMonth1Start = IsValueNull(MainFormRecord.SubTermMonth1Start) ? null : MainFormRecord.SubTermMonth1Start;
            main.SubTermMonth1End = IsValueNull(MainFormRecord.SubTermMonth1End) ? null : MainFormRecord.SubTermMonth1End;
            main.SubTermMonth2Start = IsValueNull(MainFormRecord.SubTermMonth2Start) ? null : MainFormRecord.SubTermMonth2Start;
            main.SubTermMonth2End = IsValueNull(MainFormRecord.SubTermMonth2End) ? null : MainFormRecord.SubTermMonth2End;
            main.SubTermMonth3Start = IsValueNull(MainFormRecord.SubTermMonth3Start) ? null : MainFormRecord.SubTermMonth3Start;
            main.SubTermMonth3End = IsValueNull(MainFormRecord.SubTermMonth3End) ? null : MainFormRecord.SubTermMonth3End;
            main.SubTermMonth4Start = IsValueNull(MainFormRecord.SubTermMonth4Start) ? null : MainFormRecord.SubTermMonth4Start;
            main.SubTermMonth4End = IsValueNull(MainFormRecord.SubTermMonth4End) ? null : MainFormRecord.SubTermMonth4End;
            main.NumberOfAccounts = MainFormRecord.NumberOfAccounts;

            main.GasMarketUtilityId = MainFormRecord.GasMarketUtilityId;
            main.GasRate = MainFormRecord.GasRate;
            main.GasContractTermId = MainFormRecord.GasContractTermId;
            main.GasRateEffectiveDate = MainFormRecord.GasRateEffectiveDate;

            //Get List of the OrderDetails on the MainRecord object
            List<OrderDetailFormRecord> orderdetailrecordlist = MainFormRecord.OrderDetailFormRecords;
            foreach (OrderDetailFormRecord orderdetailrecord in orderdetailrecordlist.OrderBy(x => x.OrderDetailFormRecordNumber))//make sure we add in order from 1-n for OrderDetailFormRecords
            {
                OrderDetail orderdetail = new OrderDetail();
                orderdetail.AccountNumber = orderdetailrecord.AccountNumber;
                orderdetail.GasAccountNumber = IsValueNull(orderdetailrecord.GasAccountNumber) ? null : orderdetailrecord.GasAccountNumber;
                orderdetail.MeterNumber = IsValueNull(orderdetailrecord.MeterNumber) ? null : orderdetailrecord.MeterNumber;
                orderdetail.NameKey = IsValueNull(orderdetailrecord.NameKey) ? null : orderdetailrecord.NameKey;
                orderdetail.ServiceNumber = IsValueNull(orderdetailrecord.ServiceNumber) ? null : orderdetailrecord.ServiceNumber;
                orderdetail.SubTermRate1 = IsValueNull(orderdetailrecord.SubTermRate1) ? null : orderdetailrecord.SubTermRate1;
                orderdetail.SubTermRate2 = IsValueNull(orderdetailrecord.SubTermRate2) ? null : orderdetailrecord.SubTermRate2;
                orderdetail.SubTermRate3 = IsValueNull(orderdetailrecord.SubTermRate3) ? null : orderdetailrecord.SubTermRate3;
                orderdetail.SubTermRate4 = IsValueNull(orderdetailrecord.SubTermRate4) ? null : orderdetailrecord.SubTermRate4;
                orderdetail.ServiceAddress = orderdetailrecord.ServiceAddress1;
                orderdetail.ServiceAddress2 = IsValueNull(orderdetailrecord.ServiceAddress2) ? null : orderdetailrecord.ServiceAddress2;
                orderdetail.ServiceCity = orderdetailrecord.ServiceCity;
                orderdetail.ServiceState = orderdetailrecord.ServiceState;
                orderdetail.ServiceZip = orderdetailrecord.ServiceZip;
                orderdetail.BillingAddress = orderdetailrecord.BillingAddress1;
                orderdetail.BillingAddress2 = IsValueNull(orderdetailrecord.BillingAddress2) ? null : orderdetailrecord.BillingAddress2;
                orderdetail.BillingCity = orderdetailrecord.BillingCity;
                orderdetail.BillingState = orderdetailrecord.BillingState;
                orderdetail.BillingZip = orderdetailrecord.BillingZip;

                main.OrderDetails.Add(orderdetail);
            }

            data.AddToMains(main);
            data.SaveChanges();
        }

        return main;
    }
    #endregion

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

    #endregion

    #region validation

    //Deprecated, since we are doing inline validation on main and separate modal forms, no need to validate the form and pop a menu
    //private bool PerformValidation()
    //{
    //    this.Validate();

    //    if (this.IsValid)
    //        return true;
    //    else
    //    {
    //        //Not using the list at the top of the page to show the errors
    //        //((BulletedList)this.Master.FindControl("blErrorList")).Items.Clear();

    //        //foreach (IValidator validationControl in this.Validators)
    //        //{
    //        //    validationControl.Validate();

    //        //    if (!validationControl.IsValid)
    //        //    {
    //        //        ((BulletedList)this.Master.FindControl("blErrorList")).Items.Add(validationControl.ErrorMessage);
    //        //    }
    //        //}

    //        return false;
    //    }
    //}

    #endregion

    #region Modal

    //protected void btnSaveAccount_Click(object sender, EventArgs e)
    //{

    //    //if (PerformValidation())
    //    //{

    //    //Build OrderForm Record variable

    //    //List<> of Records
    //    List<OrderDetailFormRecord> listOfOrderformrecords = new List<OrderDetailFormRecord>();
    //    //Single Record
    //    OrderDetailFormRecord orderdetailformrecord = new OrderDetailFormRecord();

    //    //grab SessionVars.OrderDetailFormRecordList
    //    if (SessionVars.OrderDetailFormRecordList != null)
    //    {
    //        listOfOrderformrecords = (List<OrderDetailFormRecord>)SessionVars.OrderDetailFormRecordList;
    //    }

    //    if (!SessionVars.AccountEditMode) // If we are in Add Mode
    //    {


    //        int OrderDetailFormRecordNumber = 0;
    //        if (listOfOrderformrecords.Count() > 0)
    //        {
    //            OrderDetailFormRecordNumber = listOfOrderformrecords.Select(v => v.OrderDetailFormRecordNumber).Last() + 1;
    //        }
    //        else
    //        {
    //            //this will be the first OrderDetailFormRecordNumber, 
    //            //but needs to be 2 since it will be added to MainFormRecord.OrderDetailFormRecord 
    //            //later which has an OrderDetailFOrmRecord object with a RecordNumber as 1 already
    //            OrderDetailFormRecordNumber = 2;
    //        }

    //        //Add a new record to the collection
    //        orderdetailformrecord.OrderDetailFormRecordNumber = OrderDetailFormRecordNumber;
    //        orderdetailformrecord.Btn = StripAllNonNumerics(txtPhoneNumber.Text.Trim());
    //        orderdetailformrecord.AccountNumber = txtModalAccountNumber.Text.Trim();
    //        orderdetailformrecord.GasAccountNumber = string.IsNullOrEmpty(txtModalGasAccountNumber.Text) ? null : txtModalGasAccountNumber.Text.Trim();
    //        orderdetailformrecord.NameKey = txtModalNameKey.Text.Trim();
    //        orderdetailformrecord.ServiceNumber = txtModalServiceNumber.Text.Trim();
    //        orderdetailformrecord.SubTermRate1 = txtModalSubTermMonth1Rate.Text.Trim();
    //        orderdetailformrecord.SubTermRate2 = txtModalSubTermMonth2Rate.Text.Trim();
    //        orderdetailformrecord.SubTermRate3 = txtModalSubTermMonth3Rate.Text.Trim();
    //        orderdetailformrecord.SubTermRate4 = txtModalSubTermMonth4Rate.Text.Trim();
    //        orderdetailformrecord.ServiceAddress1 = txtModalServiceAddress1.Text.Trim();
    //        orderdetailformrecord.ServiceAddress2 = txtModalServiceAddress2.Text.Trim();
    //        orderdetailformrecord.ServiceCity = ddlModalServiceCity.SelectedItem.Text;
    //        orderdetailformrecord.ServiceState = ddlModalServiceState.SelectedItem.Text;
    //        orderdetailformrecord.ServiceZip = txtModalServiceZip.Text.Trim();
    //        orderdetailformrecord.BillingAddress1 = txtModalBillingAddress1.Text.Trim();
    //        orderdetailformrecord.BillingAddress2 = txtModalBillingAddress2.Text.Trim();
    //        orderdetailformrecord.BillingCity = ddlModalBillingCity.SelectedItem.Text;
    //        orderdetailformrecord.BillingState = ddlModalBillingState.SelectedItem.Text;
    //        orderdetailformrecord.BillingZip = txtModalBillingZip.Text.Trim();

    //        listOfOrderformrecords.Add(orderdetailformrecord);//Add Record to orderformrecords list

    //    }
    //    else //we are in Edit Mode
    //    {

    //        //if (SessionVars.OrderDetailFormRecordList != null)
    //        //{
    //        //    orderformrecords = (List<OrderDetailFormRecord>)SessionVars.OrderDetailFormRecordList;
    //        //}
    //        //if (orderformrecords.Count() > 0)
    //        //{
    //        int OrderDetailFormRecordNumber = 0;
    //        OrderDetailFormRecordNumber = SessionVars.OrderDetailFormRecordNumber; //Get the RecordNumber we intend to edit from SessionVars
    //        foreach (OrderDetailFormRecord recordInList in listOfOrderformrecords)
    //        {
    //            if (recordInList.OrderDetailFormRecordNumber == OrderDetailFormRecordNumber)//If we are on the right Record
    //            {
    //                //update values in collection
    //                //orderformrecord.OrderDetailFormRecordNumber = OrderDetailFormRecordNumber;
    //                recordInList.Btn = StripAllNonNumerics(txtPhoneNumber.Text.Trim());
    //                recordInList.AccountNumber = txtModalAccountNumber.Text.Trim();
    //                recordInList.NameKey = txtModalNameKey.Text.Trim();
    //                recordInList.ServiceNumber = txtModalServiceNumber.Text.Trim();
    //                recordInList.SubTermRate1 = txtModalSubTermMonth1Rate.Text.Trim();
    //                recordInList.SubTermRate2 = txtModalSubTermMonth2Rate.Text.Trim();
    //                recordInList.SubTermRate3 = txtModalSubTermMonth3Rate.Text.Trim();
    //                recordInList.SubTermRate4 = txtModalSubTermMonth4Rate.Text.Trim();
    //                recordInList.ServiceAddress1 = txtModalServiceAddress1.Text.Trim();
    //                recordInList.ServiceAddress2 = txtModalServiceAddress2.Text.Trim();
    //                recordInList.ServiceCity = ddlModalServiceCity.SelectedItem.Text;
    //                recordInList.ServiceState = ddlModalServiceState.SelectedItem.Text;
    //                recordInList.ServiceZip = txtModalServiceZip.Text.Trim();
    //                recordInList.BillingAddress1 = txtModalBillingAddress1.Text.Trim();
    //                recordInList.BillingAddress2 = txtModalBillingAddress2.Text.Trim();
    //                recordInList.BillingCity = ddlModalBillingCity.SelectedItem.Text;
    //                recordInList.BillingState = ddlModalBillingState.SelectedItem.Text;
    //                recordInList.BillingZip = txtModalBillingZip.Text.Trim();
    //            }
    //        }
    //        //}
    //    }

    //    //Set the SessionVars.List<OrderDetailFormRecords> object
    //    SessionVars.OrderDetailFormRecordList = listOfOrderformrecords;

    //    //Clear out modal form controls
    //    ClearModalForm();

    //    //Close Modal Form
    //    mpePopUp.Hide();

    //    SetUpDynamicJqueryControls();

    //    //Bind Data to the GridView
    //    ShowGridView();
    //    //}
    //}
    //protected void btnCancel_Click(object sender, EventArgs e)
    //{
    //    //Clear out modal form controls
    //    ClearModalForm();

    //    //Close Modal Form
    //    mpePopUp.Hide();
    //    SetUpDynamicJqueryControls();
    //}
    //protected void chkModalMakeSameAs_CheckedChanged(object sender, EventArgs e)
    //{
    //    // Make Billing address form same as Service address form

    //    if (chkModalMakeSameAs.Checked == true)
    //    {
    //        txtModalBillingAddress1.Text = txtModalServiceAddress1.Text;
    //        txtModalBillingAddress2.Text = txtModalServiceAddress2.Text;
    //        txtModalBillingZip.Text = txtModalServiceZip.Text;

    //        ////reset Billing drop downs
    //        ResetModalBillingDropDowns();

    //        //Need to pop the dropdown lists for City and State and choose the values
    //        initModalFormFromBillingZipCodeSearch(txtModalBillingZip.Text);

    //        ddlModalBillingCity.SelectedValue = ddlModalServiceCity.SelectedValue;
    //        ddlModalBillingState.SelectedValue = ddlModalServiceState.SelectedValue;
    //    }
    //    else
    //    {
    //        txtModalBillingAddress1.Text = string.Empty;
    //        txtModalBillingAddress2.Text = string.Empty;
    //        txtModalBillingZip.Text = string.Empty;
    //        //Need to pop the dropdown lists for City and State and choose the values
    //        initModalFormFromBillingZipCodeSearch(txtModalBillingZip.Text);
    //    }
    //    //Show Modal Form
    //    mpePopUp.Show();
    //    SetUpDynamicJqueryControls();

    //}
    //protected void modalServiceZipSearch_Click(object sender, EventArgs e)
    //{
    //    string serviceZip = StripAllNonNumerics(txtModalServiceZip.Text.Trim());

    //    ZipCodeLookup zipcodelookuplist = GetZipCodeLookup(serviceZip);

    //    //reset drop downs
    //    ResetModalServiceDropDowns();

    //    if (zipcodelookuplist != null)
    //    {
    //        initModalFormFromServiceZipCodeSearch(serviceZip);
    //    }
    //    //Show Modal Form
    //    mpePopUp.Show();
    //    SetUpDynamicJqueryControls();
    //}
    //protected void modalBillingZipSearch_Click(object sender, EventArgs e)
    //{
    //    string billingZip = StripAllNonNumerics(txtModalBillingZip.Text.Trim());

    //    ZipCodeLookup zipcodelookup = GetZipCodeLookup(billingZip);

    //    //reset drop downs
    //    ResetModalBillingDropDowns();

    //    if (zipcodelookup != null)
    //    {
    //        initModalFormFromBillingZipCodeSearch(billingZip);
    //    }
    //    mpePopUp.Show();
    //    SetUpDynamicJqueryControls();
    //}
    //private void ResetModalServiceDropDowns()
    //{
    //    ddlModalServiceCity.Items.Clear();
    //    ddlModalServiceState.Items.Clear();
    //}
    //private void ResetModalBillingDropDowns()
    //{
    //    ddlModalBillingCity.Items.Clear();
    //    ddlModalBillingState.Items.Clear();
    //}
    //private void ClearModalForm()
    //{
    //    txtModalAccountNumber.Text = string.Empty;
    //    txtModalGasAccountNumber.Text = string.Empty;
    //    txtModalNameKey.Text = string.Empty;
    //    txtModalServiceNumber.Text = string.Empty; ;
    //    txtModalSubTermMonth1Rate.Text = string.Empty;
    //    txtModalSubTermMonth2Rate.Text = string.Empty;
    //    txtModalSubTermMonth3Rate.Text = string.Empty;
    //    txtModalSubTermMonth4Rate.Text = string.Empty;
    //    txtModalServiceAddress1.Text = string.Empty;
    //    txtModalServiceAddress2.Text = string.Empty;
    //    //reset drop downs
    //    ResetModalServiceDropDowns();
    //    txtModalServiceZip.Text = string.Empty;
    //    chkModalMakeSameAs.Checked = false;
    //    txtModalBillingAddress1.Text = string.Empty;
    //    txtModalBillingAddress2.Text = string.Empty;
    //    //reset drop downs
    //    ResetModalBillingDropDowns();
    //    txtModalBillingZip.Text = string.Empty;
    //}
    //private void initModalFormFromServiceZipCodeSearch(string ZipCode)
    //{
    //    //reset drop downs
    //    ddlModalServiceCity.Items.Clear();
    //    ddlModalServiceState.Items.Clear();

    //    List<string> ServiceCities = new List<string>();
    //    List<string> ServiceStates = new List<string>();


    //    using (LibertyEntities entities = new LibertyEntities())
    //    {
    //        ServiceCities = (from z in entities.ZipCodeLookups
    //                         where z.ZipCode == ZipCode
    //                         select z.City).ToList();

    //    }
    //    if (ServiceCities.Count > 0)
    //    {
    //        //populate ddlServiceCity
    //        foreach (string city in ServiceCities)
    //        {
    //            ddlModalServiceCity.Items.Add(new ListItem(city, city));
    //        }
    //        ddlModalServiceCity.Items.Insert(0, new ListItem("<-Select City->", ""));
    //    }

    //    using (LibertyEntities entities = new LibertyEntities())
    //    {
    //        ServiceStates = (from z in entities.ZipCodeLookups
    //                         where z.ZipCode == ZipCode
    //                         select z.State).Distinct().ToList();

    //    }
    //    if (ServiceStates.Count > 0)
    //    {
    //        //populate ddlServiceState
    //        foreach (string state in ServiceStates)
    //        {
    //            ddlModalServiceState.Items.Add(new ListItem(state, state));
    //        }
    //        ddlModalServiceState.Items.Insert(0, new ListItem("<-Select State->", ""));
    //    }


    //}
    //private void initModalFormFromBillingZipCodeSearch(string ZipCode)
    //{
    //    //reset drop downs
    //    ResetModalBillingDropDowns();

    //    List<string> BillingCities = new List<string>();
    //    List<string> BillingStates = new List<string>();


    //    using (LibertyEntities entities = new LibertyEntities())
    //    {
    //        BillingCities = (from z in entities.ZipCodeLookups
    //                         where z.ZipCode == ZipCode
    //                         select z.City).ToList();

    //    }
    //    //populate ddlBillingCity
    //    if (BillingCities.Count > 0)
    //    {
    //        foreach (string city in BillingCities)
    //        {
    //            ddlModalBillingCity.Items.Add(new ListItem(city, city));
    //        }
    //        ddlModalBillingCity.Items.Insert(0, new ListItem("<-Select City->", ""));
    //    }

    //    using (LibertyEntities entities = new LibertyEntities())
    //    {
    //        BillingStates = (from z in entities.ZipCodeLookups
    //                         where z.ZipCode == ZipCode
    //                         select z.State).Distinct().ToList();

    //    }
    //    if (BillingStates.Count > 0)
    //    {
    //        //populate ddlBillingState
    //        foreach (string state in BillingStates)
    //        {
    //            ddlModalBillingState.Items.Add(new ListItem(state, state));
    //        }
    //        ddlModalBillingState.Items.Insert(0, new ListItem("<-Select City->", ""));
    //    }


    //}

    #endregion

    #region Grid View
    //protected void btnAddAdditionalAccount_Click(object sender, EventArgs e)
    //{
    //    //Set ModalForm to Add mode
    //    SessionVars.AccountEditMode = false;
    //    mpePopUp.Show();
    //    SetUpDynamicJqueryControls();
    //}

    //private void ShowGridView()
    //{
    //    List<OrderDetailFormRecord> orderDetailFormRecordList = new List<OrderDetailFormRecord>();
    //    if (SessionVars.OrderDetailFormRecordList != null)
    //    {
    //        divGvAdditionalAccounts.Visible = true;
    //        orderDetailFormRecordList = (List<OrderDetailFormRecord>)SessionVars.OrderDetailFormRecordList;
    //        gvAdditionalAccounts.DataSource = orderDetailFormRecordList;
    //        gvAdditionalAccounts.DataBind();
    //    }
    //}

    //protected void gvAdditionalAccounts_RowDataBound(object sender, GridViewRowEventArgs e)
    //{
    //    //set all the columns to not word wrap
    //    for (int i = 0; i < e.Row.Cells.Count; i++)
    //    {
    //        e.Row.Cells[i].Attributes.Add("style", "white-space: nowrap; text-align: center;");
    //    }

    //}

    //protected void btnEdit_Click(object sender, CommandEventArgs e)
    //{
    //    //Get the OrderDetailFormRecordNumber from the row to edit
    //    int OrderDetailFormRecordNumber = Convert.ToInt16(e.CommandArgument.ToString());

    //    SessionVars.OrderDetailFormRecordNumber = OrderDetailFormRecordNumber;

    //    //Set Edit Flag for Modal Form submit button
    //    SessionVars.AccountEditMode = true;

    //    //Clean Modal Form
    //    ClearModalForm();


    //    //Populate Modal Form      
    //    List<OrderDetailFormRecord> orderformrecords = new List<OrderDetailFormRecord>();
    //    OrderDetailFormRecord orderdetailformrecord = new OrderDetailFormRecord();
    //    if (SessionVars.OrderDetailFormRecordList != null)
    //    {
    //        orderformrecords = (List<OrderDetailFormRecord>)SessionVars.OrderDetailFormRecordList;
    //        //Get OrderForm Record object from the List<OrderDetailFormRecord> SessionVars of which we intend to edit
    //        orderdetailformrecord = orderformrecords.Where(v => v.OrderDetailFormRecordNumber == OrderDetailFormRecordNumber).FirstOrDefault();

    //        //populate the form with the values
    //        txtModalAccountNumber.Text = orderdetailformrecord.AccountNumber;
    //        txtModalGasAccountNumber.Text = orderdetailformrecord.GasAccountNumber;
    //        txtModalNameKey.Text = orderdetailformrecord.NameKey;
    //        txtModalServiceNumber.Text = orderdetailformrecord.ServiceNumber;
    //        txtModalSubTermMonth1Rate.Text = orderdetailformrecord.SubTermRate1;
    //        txtModalSubTermMonth2Rate.Text = orderdetailformrecord.SubTermRate2;
    //        txtModalSubTermMonth3Rate.Text = orderdetailformrecord.SubTermRate3;
    //        txtModalSubTermMonth4Rate.Text = orderdetailformrecord.SubTermRate4;
    //        txtModalServiceAddress1.Text = orderdetailformrecord.ServiceAddress1;
    //        txtModalServiceAddress2.Text = orderdetailformrecord.ServiceAddress2;
    //        txtModalServiceZip.Text = orderdetailformrecord.ServiceZip;

    //        //reset Service drop downs
    //        ResetModalServiceDropDowns();

    //        //Need to pop the dropdown lists for City and State and choose the values 
    //        initModalFormFromServiceZipCodeSearch(txtModalServiceZip.Text);
    //        ddlModalServiceCity.Items.FindByText(orderdetailformrecord.ServiceCity).Selected = true;
    //        ddlModalServiceState.Items.FindByText(orderdetailformrecord.ServiceState).Selected = true;

    //        txtModalBillingAddress1.Text = orderdetailformrecord.BillingAddress1;
    //        txtModalBillingAddress2.Text = orderdetailformrecord.BillingAddress2;
    //        txtModalBillingZip.Text = orderdetailformrecord.BillingZip;

    //        //reset Billing drop downs
    //        ResetModalBillingDropDowns();

    //        //Need to pop the dropdown lists for City and State and choose the values
    //        initModalFormFromBillingZipCodeSearch(txtModalBillingZip.Text);
    //        ddlModalBillingCity.Items.FindByText(orderdetailformrecord.BillingCity).Selected = true;
    //        ddlModalBillingState.Items.FindByText(orderdetailformrecord.BillingState).Selected = true;

    //        //chkModalMakeSameAs
    //        if ((orderdetailformrecord.ServiceAddress1 == orderdetailformrecord.BillingAddress1) &&
    //            (orderdetailformrecord.ServiceAddress2 == orderdetailformrecord.BillingAddress2) &&
    //            (orderdetailformrecord.ServiceZip == orderdetailformrecord.BillingZip) &&
    //            (orderdetailformrecord.ServiceCity == orderdetailformrecord.BillingCity) &&
    //            (orderdetailformrecord.ServiceState == orderdetailformrecord.BillingState))
    //        { chkModalMakeSameAs.Checked = true; }
    //        else
    //        { chkModalMakeSameAs.Checked = false; }


    //    }

    //    //Set ModalForm to Edit mode
    //    SessionVars.AccountEditMode = true;

    //    //Show Modal Form
    //    mpePopUp.Show();
    //}

    #endregion

}





