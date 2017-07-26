using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FrontierTPVWebService
{

    /// <summary>
    /// Summary description for Tn
    /// </summary>
    public class PhoneNumber
    {

        public string Tn { get; set; }
        public bool PLOCChange { get; set; } //DialTone
        public bool PLOCFreeze { get; set; } //DialToneFreeze
        public bool ILPIntra { get; set; } //LocalToll
        public bool ILPIntraFreeze { get; set; } //LocalTollFreeze
        public bool PICInter { get; set; } //LD
        public bool PICInterFreeze { get; set; } //LDFreeze
    }
}