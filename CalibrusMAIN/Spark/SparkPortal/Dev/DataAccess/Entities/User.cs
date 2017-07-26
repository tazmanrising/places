using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calibrus.SparkPortal.DataAccess.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string AgentId { get; set; }
        public string SparkId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public string VendorNumber { get; set; }
        public int OfficeId { get; set; }
        public string OfficeName { get; set; }
    }
}
