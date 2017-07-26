using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calibrus.ClearviewPortal.DataAccess.CodeFirst.Models
{
    public class ServiceableZipCodes
    {
        [Key]
        public string ZipCodeFull { get; set; }
        public double ZIP { get; set; }
        public string ZIP_Name { get; set; }
        public double ZIP_CountyFIPS { get; set; }
        public string ZIP_County { get; set; }
        public string ZIP_State { get; set; }
        public string Utility_Name { get; set; }
        public string Holding_Company { get; set; }
        public double Utility_ID { get; set; }
        public string GAS_LDC_Type { get; set; }
        public string ELEC_Non_IOU_Type { get; set; }
        public double Percent_of_Overlap { get; set; }
        public string Utility_Territory_Type { get; set; }


    }
}
