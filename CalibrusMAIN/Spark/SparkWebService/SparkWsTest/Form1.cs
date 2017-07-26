using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SparkWsTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnInsertRecord_Click(object sender, EventArgs e)
        {
            SparkWS_localhost.Record record = new SparkWS_localhost.Record();
            //SparkWS.Record record = new SparkWS.Record();

            //These two are used to look up information for account details
            record.AgentId = "21887TML";
            record.VendorNumber = "20";
            record.Email = "";
            record.RecordLocator = "";
            record.SalesState = "IL";
            record.AuthorizationFirstName = "TEST";
            record.AuthorizationMiddle = "";
            record.AuthorizationLastName = "RECORD";
            record.Btn = "5555555551";
            record.CompanyName = "";
            record.CompanyContactFirstName = "";
            record.CompanyContactLastName = "";
            record.CompanyContactTitle = "";
            record.Territory = "";
            record.LeadType = "";
            record.Relation = "POWER OF ATTORNEY";
            //record.AccountFirstName = "TEST";
            //record.AccountLastName = "RECORD";
            record.NumberOfAccounts = "2";

            SparkWS_localhost.RecordDetail rd = new SparkWS_localhost.RecordDetail();
            //SparkWS.RecordDetail rd = new SparkWS.RecordDetail();

            //This used to look up information for account details
            rd.ProgramCode = "114";
            rd.UtilityType = "";
            rd.AccountType = "";
            rd.AccountNumber = "pe111111234111111111";
            rd.MeterNumber = "";
            rd.RateClass = "";
            rd.CustomerNameKey = "";
            rd.ServiceReferenceNumber = "";
            rd.ServiceAddress = "2246 WILLOW RD";
            rd.ServiceCity = "HOMEWOOD";
            rd.ServiceState = "MI";
            rd.ServiceZip = "60430";
            rd.BillingAddress = "2246 WILLOW RD";
            rd.BillingCity = "HOMEWOOD";
            rd.BillingState = "IL";
            rd.BillingZip = "60430";
            rd.InCityLimits = " ";
            rd.BillingFirstName = "M.B";
            rd.BillingLastName = "ROSEMON";
            rd.CustomerNameKey = "ROS ";

            SparkWS_localhost.RecordDetail rd2 = new SparkWS_localhost.RecordDetail();
            //SparkWS.RecordDetail rd2 = new SparkWS.RecordDetail();

            //This used to look up information for account details
            rd2.ProgramCode = "612";
            rd2.UtilityType = "";
            rd2.AccountType = "";
            rd2.AccountNumber = "1122445566";
            rd2.MeterNumber = "";
            rd2.RateClass = "";
            rd2.CustomerNameKey = "";
            rd2.ServiceReferenceNumber = "";
            rd2.ServiceAddress = "2246 WILLOW RD";
            rd2.ServiceCity = "HOMEWOOD";
            rd2.ServiceState = "IL";
            rd2.ServiceZip = "60430";
            rd2.BillingAddress = "2246 WILLOW RD";
            rd2.BillingCity = "HOMEWOOD";
            rd2.BillingState = "IL";
            rd2.BillingZip = "60430";
            rd2.InCityLimits = " ";
            rd2.BillingFirstName = "M.B";
            rd2.BillingLastName = "ROSEMON";

            List<SparkWS_localhost.RecordDetail> rdList = new List<SparkWS_localhost.RecordDetail>();
            //List<SparkWS.RecordDetail> rdList = new List<SparkWS.RecordDetail>();

            rdList.Add(rd);
            rdList.Add(rd2);

            record.RecordDetails = rdList.ToArray();

            string id = "0";


            //using (SparkWS.SparkWS sparkWS = new SparkWS.SparkWS())
            using (SparkWS_localhost.SparkWS sparkWS = new SparkWS_localhost.SparkWS())
            {
                id = sparkWS.SubmitRecord(record);
            }

            lblInsertRecord.Text = id.ToString();
        }

        private void btnRetrieveRecord_Click(object sender, EventArgs e)
        {
            using (SparkWS_localhost.SparkWS sparkWS = new SparkWS_localhost.SparkWS())
            {
                //SparkWS_localhost.TPVRecord tpvrecord[] = new SparkWS_localhost.TPVRecord[]{};

                var tpvrecord = sparkWS.RetrieveRecord("1/1/2015", "6/1/2015", "10");
                int count = tpvrecord.Count();
                lblRecordCount.Text = count.ToString();
            }
        }
    }
}
