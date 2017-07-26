using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calibrus.ClearviewPortal.DataAccess.Entities
{
    public class Logon
    {
        public string ClearviewId { get; set; }
        public string Password { get; set; }
        public int? UserId { get; set; }
    }
}
