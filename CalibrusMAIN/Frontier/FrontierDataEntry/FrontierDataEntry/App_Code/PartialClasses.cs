using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;

namespace FrontierModel
{
    /// <summary>
    /// Summary description for PartialClasses
    /// </summary>
    public partial class tblMain
    {
        public string PhoneNumberList
        {
            get
            {
                StringBuilder tnList = new StringBuilder();
                foreach (tblTn tn in this.tblTns)
                {
                    Regex tnFormat = new Regex(@"(\d{3})(\d{3})(\d{4})");
                    if (tnList.Length > 0)
                        tnList.Append(", ");
                    tnList.Append(tnFormat.Replace(tn.Tn, "($1) $2-$3"));
                }
                return tnList.ToString();
            }
        }

        public string CustomerName
        {
            get { return String.Format("{0} {1}", this.CustFirstName, this.CustLastName); }
        }

        public string VerifiedFormatted
        {
            get { return this.Verified == "1" ? "YES" : "NO"; }
        }
    }

    public partial class tblTn
    {
        public string TnFormatted
        {
            get
            {
                Regex tnFormat = new Regex(@"(\d{3})(\d{3})(\d{4})");
                return tnFormat.Replace(this.Tn, "($1) $2-$3");
            }
        }

        public string DialToneFormatted
        {
            get { return this.DialTone == "1" ? "YES" : "NO"; }
        }

        public string LocalTollFormatted
        {
            get { return this.LocalToll == "1" ? "YES" : "NO"; }
        }

        public string LdFormatted
        {
            get { return this.Ld == "1" ? "YES" : "NO"; }
        }

        public string DialToneFreezeFormatted
        {
            get { return this.DialToneFreeze == "1" ? "YES" : "NO"; }
        }

        public string LocalTollFreezeFormatted
        {
            get { return this.LocalTollFreeze == "1" ? "YES" : "NO"; }
        }

        public string LdFreezeFormatted
        {
            get { return this.LdFreeze == "1" ? "YES" : "NO"; }
        }

        

    }

    public partial class tblE911LoadFile
    {
        public string TnFormatted
        {
            get
            {
                Regex tnFormat = new Regex(@"(\d{3})(\d{3})(\d{4})");
                return tnFormat.Replace(this.TN, "($1) $2-$3");
            }
        }
    }


    public partial class tblSNETMain
    {
        public string PhoneNumberList
        {
            get
            {
                StringBuilder tnList = new StringBuilder();
                foreach (tblSNETTn tn in this.tblSNETTns)
                {
                    Regex tnFormat = new Regex(@"(\d{3})(\d{3})(\d{4})");
                    if (tnList.Length > 0)
                        tnList.Append(", ");
                    tnList.Append(tnFormat.Replace(tn.Tn, "($1) $2-$3"));
                }
                return tnList.ToString();
            }
        }       

        public string VerifiedFormatted
        {
            get { return this.Verified == "1" ? "YES" : "NO"; }
        }
    }

    public partial class tblSNETTn
    {
        public string TnFormatted
        {
            get
            {
                Regex tnFormat = new Regex(@"(\d{3})(\d{3})(\d{4})");
                return tnFormat.Replace(this.Tn, "($1) $2-$3");
            }
        }
        public string BTNFormatted
        {
            get { return this._Btn == "1" ? "YES" : "NO"; }
        }

        public string DialToneFormatted
        {
            get { return this.DialTone == "1" ? "YES" : "NO"; }
        }

        public string LocalTollSWBFormatted
        {
            get { return this.LocalTollSwb == "1" ? "YES" : "NO"; }
        }

        public string LocalTolSbcldFormatted
        {
            get { return this.LocalTollSbcld == "1" ? "YES" : "NO"; }
        }

        public string LDFormatted
        {
            get { return this.Ld == "1" ? "YES" : "NO"; }
        }

        public string FreezeLpicFormatted
        {
            get { return this.FreezeLpic == "1" ? "YES" : "NO"; }
        }

        public string FreezePicFormatted
        {
            get { return this.FreezePic == "1" ? "YES" : "NO"; }
        }



    }
}