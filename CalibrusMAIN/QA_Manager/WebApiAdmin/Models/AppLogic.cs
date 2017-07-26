using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiAdmin.Models
{
    public class AppLogic
    {


        public static bool PhoneNumberExists(string phone)
        {
            DateTime d = DateTime.Now.AddDays(-180);

            //using (CustomSparkPortalDataEntities ctx = new CustomSparkPortalDataEntities())
            //{
            //    MainRepository repo = new MainRepository(ctx);
            //    List<Main> main = repo.Filter(x => x.Btn == phone && x.Verified == "1" && x.CallDateTime.Value > d);
            //    return main.Count > 0;
            //}

            return true;

        }

    }
}