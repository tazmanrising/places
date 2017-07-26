using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calibrus.SparkPortal.DataAccess.Infrastructure
{
    public partial class User
    {
        public string SparkId
        {
            get
            {                
                return $"{this.Vendor?.VendorNumber??"00"}{this.Office?.OfficeId.ToString()??"000"}{this.UserId.ToString()}";
            }
        }
    }
}
