using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calibrus.ClearviewPortal.DataAccess.Infrastructure
{
    public partial class User
    {
        public string ClearviewId
        {
            get
            {
                //return $"{this.Vendor?.VendorNumber ?? "00"}{this.Office?.OfficeId.ToString() ?? "000"}{this.UserId.ToString()}";
                return $"{this.Vendor?.VendorNumber ?? "00"}{this.AgentId}";
            }
        }
    }
}
