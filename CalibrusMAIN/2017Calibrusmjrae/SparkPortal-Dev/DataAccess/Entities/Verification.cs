using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calibrus.SparkPortal.DataAccess.Entities
{
    public class Verification
    {
        public int MainId { get; set; }
        public string Verified { get; set; }
        public string Concern { get; set; }
        public string ConcernCode { get; set; }
    }
}
