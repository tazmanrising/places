using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FrontierTPVWsTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FrontierTPVService.Account account = new FrontierTPVService.Account();
            account.SalesAgentId = "063973";
            account.State = "CT";
            account.CustFirstName = "TRACY";
            account.CustLastName = "TOMCZIK";
            account.DecisionMaker = "TRACY";
            account.CompanyName = "Calibrus";
            account.Product = "?";
            account.Business = false;

            FrontierTPVService.PhoneNumber tn = new FrontierTPVService.PhoneNumber();
            tn.Tn = "8608591049";
            tn.PLOCChange = true; //DialTone
            tn.PLOCFreeze = false; //DialToneFreeze
            tn.ILPIntra = false; //LocalToll
            tn.ILPIntraFreeze = false; //LocalTollFreeze
            tn.PICInter = false; //LD
            tn.PICInterFreeze = false; //LDFreeze
            //FrontierTPVService.PhoneNumber tn2 = new FrontierTPVWsTest.FrontierTPVService.PhoneNumber();
            //tn2.Tn = "6085559449";
            //tn2.PLOCChange = false;
            //tn2.PLOCFreeze = false;
            //tn2.ILPIntra = false;
            //tn2.ILPIntraFreeze = false;
            //tn2.PICInter = false;
            //tn2.PICInterFreeze = true;

            List<FrontierTPVService.PhoneNumber> tnList = new List<FrontierTPVService.PhoneNumber>();
            tnList.Add(tn);
            //tnList.Add(tn2);

            account.PhoneNumbers = tnList.ToArray();

            int id = 0;

            using (FrontierTPVService.FrontierTPVWSSoapClient ftr = new FrontierTPVService.FrontierTPVWSSoapClient())
            {
                //ftr.ClientCredentials.UserName.UserName = @"calibrus\frontier";
                //ftr.ClientCredentials.UserName.Password = "Fr0nt13r!";
                id = ftr.SubmitOrder(account);

                label1.Text = id.ToString();
            }



        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (FrontierTPVService.FrontierTPVWSSoapClient ftr = new FrontierTPVService.FrontierTPVWSSoapClient())
            {
                DataSet status = new DataSet();
                ftr.ClientCredentials.UserName.UserName = @"calibrus\frontier";
                ftr.ClientCredentials.UserName.Password = "Fr0nt13r!";
                status = ftr.RetrieveDataTelephoneNumber(textBox1.Text.ToString());

                foreach (DataRow row in status.Tables[0].Rows)
                {
                    lblStatus.Text = row["Verified"].ToString() + "\r\n";
                    lblStatus.Text += row["Concern"].ToString() + "\r\n";
                    lblStatus.Text += row["DateTime"].ToString() + "\r\n";
                }
                //lblStatus.Text = status.Tables.Count.ToString();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (FrontierTPVService.FrontierTPVWSSoapClient ftr = new FrontierTPVService.FrontierTPVWSSoapClient())
            {
                DataSet status = new DataSet();
                ftr.ClientCredentials.UserName.UserName = @"calibrus\frontier";
                ftr.ClientCredentials.UserName.Password = "Fr0nt13r!";
                status = ftr.RetrieveDataRecordLocator(textBox2.Text.ToString());


                foreach (DataRow row in status.Tables[0].Rows)
                {
                    lblStatus.Text = row["Verified"].ToString() + "\r\n";
                    lblStatus.Text += row["Concern"].ToString() + "\r\n";
                    lblStatus.Text += row["DateTime"].ToString() + "\r\n";
                }
                //lblStatus.Text = status.Tables.Count.ToString();
            }
        }
    }
}
