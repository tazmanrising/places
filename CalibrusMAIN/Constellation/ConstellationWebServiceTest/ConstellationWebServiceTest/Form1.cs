using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ConstellationWebServiceTest.ConstellationWS;
using System.Xml;
using System.IO;


#region notes
/*

Hi Eric,
GetCustomerResponse() returns a CustomerResponse object which contains all the information about a particular response (enrollment). 

To know the signup type (Electric, Gas or Dual), please use CustomerResponse.SignUpType data member. For regular Electric and Gas signups, plan detail is stored in CustomerResponse.SelectedPlan and you can differentiate plan based on service type property as shown below. 

For dual signup, electric and gas plans are stored in different nodes. Electric plan in CustomerResponse.SelectedPlan whereas Gas plan information is in customerResponse.SelectedGasPlan. Here is how you will access these fields:

For Electric: ResponseID = 2362755

ElectricCancelFee =        CustomerResponse.SelectedPlan.PlanLegList[0].CancelFee       = 150
ElectricPrice =                   CustomerResponse.SelectedPlan.PlanLegList[0].Price1                = 12.79                  (Please note that this price includes 7% GRT tax for NJ, CustomerResponse.SelectedPlan.TaxGRTRate)
ElectricPricUOM =            CustomerResponse.SelectedPlan.PlanLegList[0].Price1UOM     = "¢/kWh"
ElectricTerm =                   CustomerResponse.SelectedPlan.PlanLegList[0].TermValue      = 6
ServiceType =                    CustomerResponse.SelectedPlan.ServiceType                                 = Electric

Now since this is NJ plan, there is another component of plan involved which is Variable term. At the end of the 6 month period, price will automatically convert to a Variable Monthly Price. 
Unless user takes action to change plans, the Variable Monthly Price structure will remain in place for at least 3 months for a total contract term of 9 months. 

So get the details on variable part of the plan, you need to access second object in PlanLegList: 

ElectricCancelFee =        CustomerResponse.SelectedPlan.PlanLegList[1].CancelFee       = 0
ElectricPrice =                   CustomerResponse.SelectedPlan.PlanLegList[1].Price1                = 15                        
(Mike/Bruce: As far as I know this price is a dummy price setup in our DB and actual variable price comes in customer bill for this 3 month period. Please confirm.)
ElectricPricUOM =            CustomerResponse.SelectedPlan.PlanLegList[1].Price1UOM     = null
ElectricTerm =                   CustomerResponse.SelectedPlan.PlanLegList[1].TermValue      = 3

For Gas: ResponseID = 2362758

GasCancelFee =                CustomerResponse.SelectedPlan.PlanLegList[0].CancelFee       = 150
GasPrice =                           CustomerResponse.SelectedPlan.PlanLegList[0].Price1                = 12.79                  (Please note that this price includes 7% GRT tax for NJ, CustomerResponse.SelectedPlan.TaxGRTRate)
GasPricUOM =                   CustomerResponse.SelectedPlan.PlanLegList[0].Price1UOM     = "¢/therm"
GasTerm=                            CustomerResponse.SelectedPlan.PlanLegList[0].TermValue      = 6 
ServiceType =                    CustomerResponse.SelectedPlan.ServiceType                                 = Gas

For Dual: ResponseID = 2362762

ElectricCancelFee =        CustomerResponse.SelectedPlan.PlanLegList[0].CancelFee       = 150
ElectricPrice =                   CustomerResponse.SelectedPlan.PlanLegList[0].Price1                = 12.79
ElectricTerm =                   CustomerResponse.SelectedPlan.PlanLegList[0].TermValue      = "¢/kWh"
FixedEnergyPrice =         
GasCancelFee =                CustomerResponse.SelectedGasPlan.PlanLegList[0].CancelFee = 150
GasPrice =                           CustomerResponse.SelectedGasPlan.PlanLegList[0].Price1        = 99.99
GasTerm =                           CustomerResponse.SelectedGasPlan.PlanLegList[0].TermValue = 6

For Texas Electric: ResponseID = 2364068

These are the plan level details and can be retrieved using function GetPlanListingBySignUpType(). This function retrieves the list of plan for a given UDC and STATE. For a given plan, attribute property can be used to get information about specific attribute. 

FixedEnergyPrice 
KWH2000Price                                  PlanListResults[0].Attributes["KWH2000PRICE"]
MinimumUsageFee                        PlanListResults[0].Attributes["MINIMUM_USAGE_FEE"]
MinimumUsageThreshold           PlanListResults[0].Attributes["MINIMUM_USAGE_THRESHOLD"]

I am not sure what FixedEnergyPrice refers to. I will get some information around it and let you know. Please let me know in case of any questions. Thanks.

Regards
Nishant

*/
#endregion

namespace ConstellationWebServiceTest
{
    public partial class Form1 : Form
    {

        //Prod
        //https://partners.constellation.com/PartnerService/ResidentialPartnerService.svc

        //Stage
        //https://partners-stage.constellation.com/PartnerService/ResidentialPartnerService.svc

        //Test
        //https://partners-test.constellation.com/PartnerService/ResidentialPartnerService.svc
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ConstellationWS.CustomerOperationResult cor = new ConstellationWS.CustomerOperationResult(); //used to get customer information
            ConstellationWS.PlanListOperationResult plor = new ConstellationWS.PlanListOperationResult(); //used to get Texas Electric information   
            ConstellationWS.AppUDCListOperationResult auor = new ConstellationWS.AppUDCListOperationResult();//used to determine which fields are required - not necessary for us


            ConstellationWS.ResidentialPartnerServiceClient rps = new ConstellationWS.ResidentialPartnerServiceClient();
            ConstellationWS.ErrorMessage em = new ConstellationWS.ErrorMessage();


            // 2362771 for Electric
            // 2362778 for Gas
            // 2362794 for dual
            cor = rps.GetCustomerResponse("CCC-NATIONAL", "CCC#2014", 3350975);
            if (cor.CustomerResponse == null)
            {
                return;

            }

            //check for Texas Electric STANDARD WAY
            //
            //plor = rps.GetPlanListingBySignUpType("CCC-NATIONAL", "CCC#2014", cor.CustomerResponse.ServiceAddress.State, cor.CustomerResponse.UDCCode, cor.CustomerResponse.PromoCode, cor.CustomerResponse.SelectedPlan.ServiceType, cor.CustomerResponse.SignUpType, cor.CustomerResponse.ServiceAddress.ZipCode);
            //var attributes = plor.PlanListResult[0].Attributes.ToList();
            //


            //check for Texas Electric Complex WAY
            //
            //Let's first see if the KWH2000Price exists in the CustomerResponse object
            var crAttributes = cor.CustomerResponse.SelectedPlan.Attributes.ToList();

            if (crAttributes.Where(p => p.Key == "KWH2000PRICE").Select(p => new { Key = p.Key, Value = p.Value }).FirstOrDefault() != null)
            {
                foreach (var pair in crAttributes) //Get the values from the CustomerResponse object
                {
                    if (pair.Key == "KWH2000PRICE")
                    {
                        label1.Text += "KWH2000PRICE:" + pair.Value + ",";
                    }
                    if (pair.Key == "MINIMUM_USAGE_FEE")
                    {
                        label1.Text += "MINIMUM_USAGE_FEE:" + pair.Value + ",";
                    }
                    if (pair.Key == "MINIMUM_USAGE_THRESHOLD")
                    {
                        label1.Text += "MINIMUM_USAGE_THRESHOLD:" + pair.Value + ",";
                    }
                }
            }
            else
            {
                plor = rps.GetPlanListingBySignUpType("CCC-NATIONAL", "CCC#2014", cor.CustomerResponse.ServiceAddress.State, cor.CustomerResponse.UDCCode, cor.CustomerResponse.PromoCode, cor.CustomerResponse.SelectedPlan.ServiceType, cor.CustomerResponse.SignUpType, cor.CustomerResponse.ServiceAddress.ZipCode);
                var attributes = plor.PlanListResult[0].Attributes.ToList();

                foreach (var pair in attributes)
                {
                    if (pair.Key == "KWH2000PRICE")
                    {
                        label1.Text += "KWH2000PRICE:" + pair.Value + ",";
                    }
                    if (pair.Key == "MINIMUM_USAGE_FEE")
                    {
                        label1.Text += "MINIMUM_USAGE_FEE:" + pair.Value + ",";
                    }
                    if (pair.Key == "MINIMUM_USAGE_THRESHOLD")
                    {
                        label1.Text += "MINIMUM_USAGE_THRESHOLD:" + pair.Value + ",";
                    }
                }
            }
            //


            //used to test production
            //rps.GetPlanListing("CCC-NATIONAL", "CCC#2014","Ga","AGL",promocode)
            //used to test production


            //cor = rps.UpdateTPVVerificationCode()

            auor = rps.GetUDCListByType("CCC-NATIONAL", "CCC#2014", "NJ", SignUpChoiceEnum.Dual);

            //Used to find specific values from Texas Electric
            plor = rps.GetPlanListingBySignUpType("CCC-NATIONAL", "CCC#2014", cor.CustomerResponse.ServiceAddress.State, cor.CustomerResponse.UDCCode, cor.CustomerResponse.PromoCode, cor.CustomerResponse.SelectedPlan.ServiceType, cor.CustomerResponse.SignUpType, cor.CustomerResponse.ServiceAddress.ZipCode);

            label1.Text = cor.CustomerResponse.UDCCode.ToString();


            //upate email
            //cor = rps.SaveCustomerSignUpByType("CCC-NATIONAL", "CCC#2014", null);//update record

            //Update status
            //em[] = rps.UpdateTPVStatus("CCC-NATIONAL", "CCC#2014",2362794,TPVStatusEnum.TPVNonVerified,"concern");

            //write to xml file
            System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(cor.GetType());
            XmlWriter writer = XmlWriter.Create(@"c:\ConstellationResponse.xml");
            xs.Serialize(writer, cor);
            writer.Close();

            //var ds = new DataSet();
            //var path = Path.GetFullPath(@"c:\ConstellationResponse.xml");
            //ds.ReadXml(path);
            //dataGridView1.DataSource = ds.Tables[0].DefaultView;
            //dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
            //dataGridView1.Refresh();


            rps.Close();

        }

        private void btnTestResRenewal_Click(object sender, EventArgs e)
        {
            ConstellationWS.CustomerOperationResult cor = new ConstellationWS.CustomerOperationResult(); //used to get customer information
            ConstellationWS.PlanListOperationResult plor = new ConstellationWS.PlanListOperationResult(); //used to get Texas Electric information   
            ConstellationWS.AppUDCListOperationResult auor = new ConstellationWS.AppUDCListOperationResult();//used to determine which fields are required - not necessary for us


            ConstellationWS.ResidentialPartnerServiceClient rps = new ConstellationWS.ResidentialPartnerServiceClient();
            ConstellationWS.ErrorMessage em = new ConstellationWS.ErrorMessage();


            // 3091396 
            // 3091398
            // 3091399
            // 3091403 
            // for Electric
            // 2362778 for Gas
            // 2362794 for dual
            cor = rps.GetCustomerRenewalResponse("CCC-NATIONAL", "CCC#2014", 2362778);
            if (cor.CustomerResponse == null)
            {
                return;

            }

            //check for Texas Electric STANDARD WAY
            //
            //plor = rps.GetPlanListingBySignUpType("CCC-NATIONAL", "CCC#2014", cor.CustomerResponse.ServiceAddress.State, cor.CustomerResponse.UDCCode, cor.CustomerResponse.PromoCode, cor.CustomerResponse.SelectedPlan.ServiceType, cor.CustomerResponse.SignUpType, cor.CustomerResponse.ServiceAddress.ZipCode);
            //var attributes = plor.PlanListResult[0].Attributes.ToList();
            //


            //check for Texas Electric Complex WAY
            //
            //Let's first see if the KWH2000Price exists in the CustomerResponse object
            var crAttributes = cor.CustomerResponse.SelectedPlan.Attributes.ToList();

            if (crAttributes.Where(p => p.Key == "KWH2000PRICE").Select(p => new { Key = p.Key, Value = p.Value }).FirstOrDefault() != null)
            {
                foreach (var pair in crAttributes) //Get the values from the CustomerResponse object
                {
                    if (pair.Key == "KWH2000PRICE")
                    {
                        label1.Text += "KWH2000PRICE:" + pair.Value + ",";
                    }
                    if (pair.Key == "MINIMUM_USAGE_FEE")
                    {
                        label1.Text += "MINIMUM_USAGE_FEE:" + pair.Value + ",";
                    }
                    if (pair.Key == "MINIMUM_USAGE_THRESHOLD")
                    {
                        label1.Text += "MINIMUM_USAGE_THRESHOLD:" + pair.Value + ",";
                    }
                }
            }
            else
            {
                plor = rps.GetPlanListingBySignUpType("CCC-NATIONAL", "CCC#2014", cor.CustomerResponse.ServiceAddress.State, cor.CustomerResponse.UDCCode, cor.CustomerResponse.PromoCode, cor.CustomerResponse.SelectedPlan.ServiceType, cor.CustomerResponse.SignUpType, cor.CustomerResponse.ServiceAddress.ZipCode);
                var attributes = plor.PlanListResult[0].Attributes.ToList();

                foreach (var pair in attributes)
                {
                    if (pair.Key == "KWH2000PRICE")
                    {
                        label1.Text += "KWH2000PRICE:" + pair.Value + ",";
                    }
                    if (pair.Key == "MINIMUM_USAGE_FEE")
                    {
                        label1.Text += "MINIMUM_USAGE_FEE:" + pair.Value + ",";
                    }
                    if (pair.Key == "MINIMUM_USAGE_THRESHOLD")
                    {
                        label1.Text += "MINIMUM_USAGE_THRESHOLD:" + pair.Value + ",";
                    }
                }
            }
            //


            //used to test production
            //rps.GetPlanListing("CCC-NATIONAL", "CCC#2014","Ga","AGL",promocode)
            //used to test production


            //cor = rps.UpdateTPVVerificationCode()

            auor = rps.GetUDCListByType("CCC-NATIONAL", "CCC#2014", "NJ", SignUpChoiceEnum.Dual);

            //Used to find specific values from Texas Electric
            plor = rps.GetPlanListingBySignUpType("CCC-NATIONAL", "CCC#2014", cor.CustomerResponse.ServiceAddress.State, cor.CustomerResponse.UDCCode, cor.CustomerResponse.PromoCode, cor.CustomerResponse.SelectedPlan.ServiceType, cor.CustomerResponse.SignUpType, cor.CustomerResponse.ServiceAddress.ZipCode);

            label2.Text = cor.CustomerResponse.UDCCode.ToString();


            //upate email
            //cor = rps.SaveCustomerSignUpByType("CCC-NATIONAL", "CCC#2014", null);//update record

            //Update status
            //em[] = rps.UpdateRenewalTPVStatus("CCC-NATIONAL", "CCC#2014",2362794,TPVStatusEnum.TPVNonVerified,"concern");

            //write to xml file
            System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(cor.GetType());
            XmlWriter writer = XmlWriter.Create(@"c:\ConstellationResResponse.xml");
            xs.Serialize(writer, cor);
            writer.Close();

            //var ds = new DataSet();
            //var path = Path.GetFullPath(@"c:\ConstellationResponse.xml");
            //ds.ReadXml(path);
            //dataGridView1.DataSource = ds.Tables[0].DefaultView;
            //dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
            //dataGridView1.Refresh();


            rps.Close();

        }

        /// <summary>
        /// Used to get zip code lists to import into Constellation.HomeServicesZipCodeLookUp table.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTestHomeServices_Click(object sender, EventArgs e)
        {
            ConstellationWS.CustomerOperationResult cor = new ConstellationWS.CustomerOperationResult(); //used to get customer information
            ConstellationWS.PlanListOperationResult plor = new ConstellationWS.PlanListOperationResult(); //used to get Texas Electric information   
            ConstellationWS.AppUDCListOperationResult auor = new ConstellationWS.AppUDCListOperationResult();//used to determine which fields are required - not necessary for us
            ConstellationWS.UdcZipCodesListResult udcr = new ConstellationWS.UdcZipCodesListResult();//used to get UDCZipCode Result Information

            ConstellationWS.ResidentialPartnerServiceClient rps = new ConstellationWS.ResidentialPartnerServiceClient();
            ConstellationWS.ErrorMessage em = new ConstellationWS.ErrorMessage();
            

            //TX, MD, PA 
            udcr = rps.GetUdcZipCodes("CCC-NATIONAL", "CCC#2014", "PA", ServiceTypeEnum.HomeServices);
            if (udcr.ZipCodesByUdc == null)
            {
                return;

            }
            var ziplist = udcr.ZipCodesByUdc.ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var item in ziplist)
            {
              
              foreach (var zip in item.ZipCodes)
              {
                  sb.AppendLine(item.UdcCode + "," + item.UdcName + "," + zip  );//builds comma delimited list to insert into homeserviceszipcodelookup, you must manually import by saving to a text file
              }
            }

            rps.Close();
        }




    }
}
