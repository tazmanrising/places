using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calibrus.ClearviewPortal.DataAccess.CodeFirst.Models;
using Calibrus.ClearviewPortal.DataAccess.Infrastructure;
using Calibrus.ClearviewPortal.DataAccess.Repository;
using Calibrus.ClearviewPortal.DataAccess.Utilities;

namespace Calibrus.ClearviewPortal.DataAccess.CodeFirst
{
    public class ZipCodeService
    {
        public static List<ServiceableZipCodes> GetServiceableZipCodes(string zipCode)
        {
      
            try
            {
                using (var ctx = new ClearviewContext())
                {
                        var query = ctx.ServiceableZipCodeses
                           .Where(z => z.ZipCodeFull == zipCode)
                           .AsEnumerable()
                           .Select(z => new ServiceableZipCodes()
                           {
                               ZipCodeFull = z.ZipCodeFull,
                               ZIP = z.ZIP,
                               ELEC_Non_IOU_Type = z.ELEC_Non_IOU_Type,
                               GAS_LDC_Type = z.GAS_LDC_Type,
                               Holding_Company = z.Holding_Company,
                               Percent_of_Overlap = z.Percent_of_Overlap,
                               Utility_ID = z.Utility_ID,
                               Utility_Name = z.Utility_Name,
                               Utility_Territory_Type = z.Utility_Territory_Type,
                               ZIP_County = z.ZIP_County,
                               ZIP_CountyFIPS = z.ZIP_CountyFIPS,
                               ZIP_Name = z.ZIP_Name,
                               ZIP_State = z.ZIP_State
                          
                           }).ToList();
                    
                        return query;
                }
            }
            catch (Exception e)
            {
                var x = e.Message;
                throw;
            }

            
        }
    }
}
